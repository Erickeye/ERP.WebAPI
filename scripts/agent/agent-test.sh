#!/usr/bin/env bash
# 在 agent 的 Linux VM 離線還原並執行 ERP.Tests。用法：bash scripts/agent/agent-test.sh [dotnet test 參數...]
# 例：bash scripts/agent/agent-test.sh --filter "FullyQualifiedName~BusinessTrip"
# 不會在專案資料夾內 restore 或 build；詳見 docs/agents/build-and-test.md。
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
export PATH="$HOME/.dotnet:$PATH" DOTNET_CLI_HOME="$HOME" DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

build="${AGENT_BUILD_DIR:-$HOME/build}"
cache="${NUGET_CACHE_DIR:-$HOME/mnt/packages}"
# 直接把 NuGet 全域套件資料夾指到掛載的快取：不再複製約 2GB 到 VM，restore 找得到套件就幾乎不花時間
export NUGET_PACKAGES="$cache"

t0=$(date +%s); tlast=$t0
stage() { local now; now=$(date +%s); echo "[計時] $1：$((now - tlast)) 秒" >&2; tlast=$now; }

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
stage "確認 dotnet"

mkdir -p "$build"
changes="$(rsync -a --delete -i \
  --exclude .vs --exclude '*.dbmdl' --exclude '*.jfm' --exclude obj --exclude bin \
  --exclude node_modules --exclude .git --exclude dbData --exclude ERP.Vue \
  --exclude .scratch --exclude .claude --exclude document --exclude docs --exclude scripts \
  --exclude /NuGet.config \
  "$repo/" "$build/" | grep -E '^>f|^\*deleting' || true)"
stage "rsync 同步專案"

cat > "$build/NuGet.config" <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration><packageSources><clear/><add key="local" value="$cache"/></packageSources></configuration>
XML

cd "$build"
proj=src/ERP.Tests/ERP.Tests.csproj
dll=src/ERP.Tests/bin/Debug/net8.0/ERP.Tests.dll

# 沒有任何檔案變動且已編譯過：直接跑測試
if [ -z "$changes" ] && [ -f "$dll" ]; then
  echo "[略過] 專案檔案沒有變動，不重新 restore 與編譯" >&2
else
  # 專案檔、套件設定沒變且已 restore 過：略過 restore（約省 10~20 秒）
  if [ -f src/ERP.Tests/obj/project.assets.json ] && ! echo "$changes" | grep -qE '\.csproj|\.props|\.targets|NuGet\.config|packages\.lock'; then
    echo "[略過] 專案檔沒有變動，不重新 restore" >&2
  else
    dotnet restore "$proj" --configfile "$build/NuGet.config" -v q
  fi
  stage "restore"
  dotnet build "$proj" --no-restore -v q -nologo
  stage "編譯"
fi

rc=0
dotnet test "$proj" --no-build "$@" || rc=$?
stage "執行測試"
echo "[計時] 合計：$(( $(date +%s) - t0 )) 秒" >&2
exit $rc
