#!/usr/bin/env bash
# 在 agent 的 Linux VM 驗證前端能否建置（vite build）。用法：bash scripts/agent/agent-vue-build.sh
# 在暫存副本建置，不動專案資料夾的 node_modules；建置完成後刪除 dist 以節省磁碟。
set -euo pipefail

repo="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
work="${AGENT_VUE_DIR:-$HOME/vue-build}"

mkdir -p "$work"
rsync -a --delete --exclude node_modules --exclude dist "$repo/src/ERP.Vue/" "$work/"
cd "$work"
[ -d node_modules ] || npm ci --no-audit --no-fund
npm run build
rm -rf dist
echo "前端建置通過（未在瀏覽器實際操作）。"
