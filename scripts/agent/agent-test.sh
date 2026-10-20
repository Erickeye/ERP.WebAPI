#!/usr/bin/env bash
# 在 agent 的 Linux VM 離線還原並執行 ERP.Tests。用法：bash scripts/agent/agent-test.sh [dotnet test 參數...]
# 例：bash scripts/agent/agent-test.sh --filter "FullyQualifiedName~BusinessTrip"
# 不會在專案資料夾內 restore 或 build；詳見 docs/agents/build-and-test.md。
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
export PATH="$HOME/.dotnet:$PATH" DOTNET_CLI_HOME="$HOME" DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

build="${AGENT_BUILD_DIR:-$HOME/build}"
cache="${NUGET_CACHE_DIR:-$HOME/mnt/packages}"

if [ ! -d "$cache" ]; then
  echo "找不到 NuGet 快取：$cache" >&2
  echo "請先向使用者要求讀取 C:\\Users\\<使用者>\\.nuget\\packages 的授權（僅讀取），掛載後會出現在 \$HOME/mnt/packages。" >&2
  exit 2
fi

avail_mb="$(df -Pm "$HOME" | awk 'NR==2{print $4}')"
if [ "${avail_mb:-0}" -lt 1500 ]; then
  echo "警告：磁碟剩 ${avail_mb}MB，低於 1500MB；請先刪除不用的暫存目錄（例如舊的 \$HOME/build-*、\$HOME/vue-build/dist）。" >&2
fi

bash "$repo/scripts/agent/ensure-dotnet.sh"

mkdir -p "$build"
rsync -a --delete \
  --exclude .vs --exclude '*.dbmdl' --exclude '*.jfm' --exclude obj --exclude bin \
  --exclude node_modules --exclude .git --exclude dbData --exclude ERP.Vue \
  --exclude .scratch --exclude .claude --exclude document --exclude docs --exclude scripts \
  "$repo/" "$build/"

cat > "$build/NuGet.config" <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration><packageSources><clear/><add key="local" value="$cache"/></packageSources></configuration>
XML

cd "$build"
dotnet restore src/ERP.Tests/ERP.Tests.csproj --configfile "$build/NuGet.config" -v q
dotnet test src/ERP.Tests/ERP.Tests.csproj --no-restore "$@"
