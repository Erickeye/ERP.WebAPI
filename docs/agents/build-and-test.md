# 編譯與測試（agent 工作階段）

agent 的工作階段在 Linux VM 執行，網路預設無法連到 `api.nuget.org`（403）與 dot.net，所以不能線上 restore，也不能用官方腳本裝 .NET。**不要在使用者的專案資料夾內執行 restore 或 build**，否則會改寫 Visual Studio 產生的 `obj/`。

## 指令

```
bash scripts/agent/agent-test.sh [dotnet test 參數]     # 離線還原並執行 ERP.Tests
bash scripts/agent/agent-vue-build.sh                    # 驗證前端 vite build
```

`agent-test.sh` 依序：確認 NuGet 快取已掛載、必要時安裝 .NET 8 SDK（`ensure-dotnet.sh`，從 Ubuntu 套件庫取 .deb 到 `$HOME/.dotnet`）、把專案同步到 `$HOME/build`、產生只含快取的 `NuGet.config`、離線 restore、`dotnet test`。修改一律寫回專案資料夾，`$HOME/build` 只用來編譯測試。

## 前置條件

- 向使用者要求讀取 `C:\Users\<使用者>\.nuget\packages` 的授權（僅讀取，該資料夾是 Visual Studio 還原過的套件快取），掛載後出現在 `$HOME/mnt/packages`。沒有授權時腳本會停下並提示。
- 磁碟配額有限（約 10GB，一份編譯暫存約 1GB、.NET SDK 約 0.5GB）。腳本在剩餘不足 1500MB 時會警告；用完不需要的暫存（舊的 `$HOME/build-*`、前端的 `dist`）請刪除。

## 限制

- 快取裡沒有的套件（新專案、新增或升級後 Visual Studio 還沒還原過的版本）無法離線還原，需使用者先在 Visual Studio 還原一次。
- 需要與專案相同主版本的 .NET SDK（目前 net8.0）。
- Windows 專用專案（WinForms、WPF、.NET Framework）無法在此編譯。
- 測試使用 SQLite 記憶體庫，不等於 SQL Server：decimal 排序與彙總、`rowversion`、`getdate()` 預設值行為不同。列表查詢、匯出與預存程序必須由使用者在真實資料庫驗證。
- 前端只驗證能建置，不等於畫面可用；沒有前端自動化測試。
