# AGENTS.md

- 回答請使用繁體中文。
- 前端 Vue、JavaScript、API wrapper、Router、Element Plus 頁面與前端 CSS 工作，請使用 `.claude/skills/vue-coding-style`。
- 後端 API、Controller、Service、ViewModel、EntityModels、DB、Helper、Extension、Swagger、驗證、Log、授權、EF Core 查詢、排序、分頁、簽核模組相關工作，請使用 `.claude/skills/backend-coding-style`。
- Code Review（review 功能、commit、branch、PR、工作樹）請使用 `.claude/skills/code-review`；其 Standards 軸的規範來源為 `document/codingStyle.md`、`document/vueCodingStyle.md` 與上述兩個 coding-style skill。
- 本專案後端 coding style 來源為 `document/codingStyle.md`；若 skill 內容與此檔不同，以 `document/codingStyle.md` 為準。
- 本專案前端 coding style 來源為 `document/vueCodingStyle.md`；若 skill 內容與此檔不同，以 `document/vueCodingStyle.md` 為準。
- 文件中標示「現況落差」的項目，代表既有程式尚未符合規範：新程式依規範撰寫，既有程式只在被修改到該區域時順手收斂，不主動大範圍重構。
- 不得將真實密碼、金鑰、連線字串提交進 repo；本機設定放 `appsettings.Development.json`，範例用 `appsettings.Development.example.json` 佔位。
- `document/補強路線圖.md` 記錄本專案尚待補齊的文件、skill 與技術項目，開始新的規範性工作前可先參考。
- `document/backup/` 為已封存的舊文件（含舊的 `reviewPrompt.md` 與 `feature-review` skill），不屬於現行規範，Code Review 與其他 skill 搜尋規範時請一律忽略。

## Agent skills

### Issue tracker

Issues and specs live as local markdown files under `.scratch/`. See `docs/agents/issue-tracker.md`.

### Domain docs

Single-context: one `GLOSSARY.md` and `docs/adr/` at the repo root. See `docs/agents/domain.md`.

## 編譯與測試（agent 工作階段）

agent 的工作階段在 Linux 環境執行，網路預設無法連到 `api.nuget.org`，所以線上 `dotnet restore` 會失敗（403）。請改用「離線還原」，並且**不要在使用者的專案資料夾內執行 restore 或 build**，否則會改寫 Visual Studio 產生的 `obj/`。

1. 向使用者要求讀取 `C:\Users\<使用者>\.nuget\packages` 的授權（僅讀取，該資料夾是 Visual Studio 還原過的套件快取）。
2. 把專案複製到工作階段的暫存目錄（例如 `$HOME/build`），排除 `obj`、`bin`、`node_modules`、`.git`、`dbData`、`ERP.Vue`。
3. 在暫存目錄放一份只含快取的 `NuGet.config`（`<clear/>` 後只加一個指向快取資料夾的 local 來源），執行 `dotnet restore <測試專案> --configfile <該檔>`。
4. 執行 `dotnet test <測試專案> --no-restore`。
5. 修改要回到專案資料夾，暫存目錄只用來編譯與測試。

限制：
- 快取裡沒有的套件（新專案、新增或升級後 Visual Studio 還沒還原過的版本）無法離線還原，需使用者先在 Visual Studio 還原一次。
- 需要與專案相同主版本的 .NET SDK；工作階段內沒有時需自行安裝到使用者目錄。
- Windows 專用專案（WinForms、WPF、.NET Framework）無法在此編譯。
- 測試使用 SQLite 記憶體庫，不等於 SQL Server：decimal 排序與彙總、`rowversion`、`getdate()` 預設值行為不同。列表查詢、匯出與預存程序必須由使用者在真實資料庫驗證。
- 不執行 `git commit` 與 `git push`：在專案資料夾內 commit 需要刪除權限（會留下鎖定檔），push 需要使用者的 git 憑證。commit 與 push 由使用者在 Visual Studio 完成；`code-review` 需要至少一個 commit 才有差異可比對。
