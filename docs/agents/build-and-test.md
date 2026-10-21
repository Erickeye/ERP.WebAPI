# 編譯與測試（agent 工作階段）

agent 的工作階段在 Linux VM 執行，網路預設無法連到 `api.nuget.org`（403）與 dot.net，所以不能線上 restore，也不能用官方腳本裝 .NET。**不要在使用者的專案資料夾內執行 restore 或 build**，否則會改寫 Visual Studio 產生的 `obj/`。

## 指令

```
bash scripts/agent/agent-test.sh [dotnet test 參數]     # 離線還原並執行 ERP.Tests
bash scripts/agent/agent-vue-build.sh                    # 驗證前端 vite build
```

`agent-test.sh` 依序：確認 NuGet 快取已掛載、必要時安裝 .NET 8 SDK（`ensure-dotnet.sh`，從 Ubuntu 套件庫取 .deb 到 `$HOME/.dotnet`）、用 rsync 把專案同步到 `$HOME/build`（只傳有變動的檔案）、restore、編譯、`dotnet test`。修改一律寫回專案資料夾，`$HOME/build` 只用來編譯測試。

## 速度與省時做法

- 腳本把 `NUGET_PACKAGES` 直接指到掛載的快取，不再複製套件到 VM（以前會多佔約 2GB 磁碟）。
- 沒有任何檔案變動且已編譯過：略過 restore 與編譯，直接跑測試。
- 只有程式碼變動（`.csproj`、`.props`、`NuGet.config` 沒變）：略過 restore。
- 每個階段會印出 `[計時]`，要查哪一段慢就看它。
- 實測參考（2 核心 VM）：無變動約 40 秒；有程式碼變動約 110 秒；冷啟動（含完整編譯）約 150 秒以上。MSBuild 即使 `CoreCompile` 全部跳過，評估與專案參考解析仍約 50 秒，這是這台 VM 的下限，無法再用腳本壓縮。
- 因此：tdd 中間只用 `--filter` 跑相關測試類別，全套測試只在 ticket 結尾跑一次；不要為了小改動重複全套。
- 單一指令最多 180 秒：把輸出導到檔案再用 `tail`／`grep` 讀（`timeout 170 bash scripts/agent/agent-test.sh ... > $HOME/tmp/run.log 2>&1`），逾時後 `obj/` 的進度仍會保留，重跑會接續。

## 前置條件

- 向使用者要求讀取 `C:\Users\<使用者>\.nuget\packages` 的授權（僅讀取，該資料夾是 Visual Studio 還原過的套件快取），掛載後出現在 `$HOME/mnt/packages`。沒有授權時腳本會停下並提示。
- 磁碟配額有限（約 10GB，一份編譯暫存約 1GB、.NET SDK 約 0.5GB）。腳本在剩餘不足 1500MB 時會警告；用完不需要的暫存（舊的 `$HOME/build-*`、`$HOME/vue-*`、`$HOME/debs`、前端的 `dist`）請刪除；不要用 `du` 掃掛載的 `$HOME/mnt/packages`，檔案太多會逾時。

## 限制

- 快取裡沒有的套件（新專案、新增或升級後 Visual Studio 還沒還原過的版本）無法離線還原，需使用者先在 Visual Studio 還原一次。
- 需要與專案相同主版本的 .NET SDK（目前 net8.0）。
- Windows 專用專案（WinForms、WPF、.NET Framework）無法在此編譯。
- 測試使用 SQLite 記憶體庫，不等於 SQL Server：decimal 排序與彙總、`rowversion`、`getdate()` 預設值行為不同。列表查詢、匯出與預存程序必須由使用者在真實資料庫驗證。
- 前端只驗證能建置，不等於畫面可用；沒有前端自動化測試。
