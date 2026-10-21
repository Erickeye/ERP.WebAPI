# 編譯與測試（Windows 本機）

agent 在使用者的 Windows 本機執行（Claude Code），直接使用本機的 .NET SDK、NuGet 與 Node.js，可以連線 restore，不需要額外腳本。

## 指令

在專案根目錄執行：

```
dotnet test src/ERP.Tests/ERP.Tests.csproj                                   # 全套後端測試
dotnet test src/ERP.Tests/ERP.Tests.csproj --filter "FullyQualifiedName~Supplier"   # 只跑相關測試類別
dotnet build ERP.WebAPI.sln                                                  # 只確認能編譯
```

前端驗證能建置：

```
cd src/ERP.Vue
npm ci          # node_modules 不存在或 package-lock.json 變動時才需要
npm run build
```

## 注意事項

- 專案目標是 `net8.0`，需要本機有 .NET 8 Runtime（本機另有 9、10 SDK 時，用較新的 SDK 也能編譯 net8.0）。
- tdd 中間只用 `--filter` 跑相關測試類別，全套測試只在 ticket 結尾跑一次。
- 直接在專案資料夾編譯會改寫 `obj/`、`bin/`。Visual Studio 開著時可能提示專案在外部被修改，重新載入即可，不影響原始碼。
- 前端 `npm run build` 會產生 `src/ERP.Vue/dist`；它已被 git 忽略，不需要提交。
- 輸出很長時，用 `| tail -n 50` 或 `grep` 只看失敗與摘要。

## 限制

- 測試使用 SQLite 記憶體庫，不等於 SQL Server：decimal 排序與彙總、`rowversion`、`getdate()` 預設值行為不同。列表查詢、匯出與預存程序必須由使用者在真實資料庫驗證。
- 前端只驗證能建置，不等於畫面可用；沒有前端自動化測試。
