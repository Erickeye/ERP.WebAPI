# 編譯與測試

agent 的工作階段在 Linux VM 執行：磁碟額度小、網路受限（連不上 `api.nuget.org`）、沒有 .NET，也不穩定，所以**agent 不編譯、不跑測試**。編譯與測試由使用者在自己的電腦執行，agent 只做靜態檢查（讀檔、`rg`、diff review）。

## 分工

- agent：寫測試與實作，交付時說明「僅完成靜態檢查，尚未執行測試」，並列出要使用者跑的指令。
- 使用者：在本機執行，並回報結果（通過 / 失敗訊息）。

## 使用者要跑的指令

```
dotnet test src/ERP.Tests/ERP.Tests.csproj                                  # 全套
dotnet test src/ERP.Tests/ERP.Tests.csproj --filter "FullyQualifiedName~BusinessTrip"   # 只跑相關測試
cd src/ERP.Vue && npm run build                                              # 前端建置
```

也可以在 Visual Studio 的 Test Explorer 執行。

## 與 tdd、implement 的搭配

- tdd 的「紅 → 綠」在每個切片寫完測試與實作後暫停，請使用者執行對應的 `--filter` 並回報；不要連續做多個切片後才一次回報。
- 全套測試只在 ticket 結尾請使用者跑一次。
- 使用者回報失敗時，依訊息修正再請使用者重跑，不要猜測結果。

## 限制

- 測試使用 SQLite 記憶體庫，不等於 SQL Server：decimal 排序與彙總、`rowversion`、`getdate()` 預設值行為不同。列表查詢、匯出與預存程序必須由使用者在真實資料庫驗證。
- 前端只驗證能建置，不等於畫面可用；沒有前端自動化測試。
