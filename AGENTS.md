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
