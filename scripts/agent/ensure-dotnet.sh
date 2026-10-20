#!/usr/bin/env bash
# 在 agent 的 Linux VM 安裝 .NET 8 SDK 到 $HOME/.dotnet（不需要 root）。
# dot.net 與 Microsoft 的下載網域被 proxy 擋（403），所以改從 Ubuntu 套件庫（security 與 updates 的 main）取 .deb 解壓。
# 已安裝 8.x SDK 時直接結束。版本由套件索引決定，不寫死。
set -euo pipefail

root="${DOTNET_ROOT:-$HOME/.dotnet}"
if [ -x "$root/dotnet" ] && "$root/dotnet" --list-sdks 2>/dev/null | grep -q '^8\.'; then
  echo "dotnet 已安裝：$("$root/dotnet" --version)（$root）"
  exit 0
fi

codename="$(. /etc/os-release && echo "${VERSION_CODENAME:-}")"
[ -n "$codename" ] || { echo "無法判斷 Ubuntu 版本代號" >&2; exit 1; }
mirror="${UBUNTU_MIRROR:-http://archive.ubuntu.com/ubuntu}"
packages=(dotnet-host-8.0 dotnet-hostfxr-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0
          dotnet-targeting-pack-8.0 aspnetcore-targeting-pack-8.0 dotnet-apphost-pack-8.0
          netstandard-targeting-pack-2.1-8.0 dotnet-templates-8.0 dotnet-sdk-8.0)

work="$(mktemp -d "$HOME/dotnet-install.XXXXXX")"
trap 'rm -rf "$work"' EXIT

# dotnet 套件在 <代號>-security/main 與 <代號>-updates/main，兩個索引都查，取最新版本
: > "$work/table"
for suite in "${codename}-security" "${codename}-updates"; do
  echo "下載套件索引（$suite/main）..."
  curl -fsSL "$mirror/dists/$suite/main/binary-amd64/Packages.gz" | gunzip |
    awk '$1=="Package:"{p=$2} $1=="Version:"{v=$2} $1=="Filename:"{print p, v, $2}' >> "$work/table"
done

mkdir -p "$work/x" "$root"
for pkg in "${packages[@]}"; do
  file="$(grep -E "^$pkg " "$work/table" | sort -k2,2V | tail -1 | awk '{print $3}')"
  [ -n "$file" ] || { echo "套件索引找不到 $pkg" >&2; exit 1; }
  echo "安裝 ${file##*/}"
  curl -fsSL -o "$work/pkg.deb" "$mirror/$file"
  dpkg -x "$work/pkg.deb" "$work/x"
  rm -f "$work/pkg.deb"
done

cp -a "$work/x/usr/lib/dotnet/." "$root/"
echo "完成：$("$root/dotnet" --version)（$root）"
