# AGENTS.md

- 回答請使用繁體中文。
- 前端 Vue、JavaScript、API wrapper、Router、Element Plus 頁面與前端 CSS 工作，請使用 `.claude/skills/vue-coding-style`。
- 後端 API、Controller、Service、ViewModel、EntityModels、DB、Helper、Extension、Swagger、驗證、Log、授權、EF Core 查詢、排序、分頁、簽核模組相關工作，請使用 `.claude/skills/backend-coding-style`。
- Code Review（review 功能、commit、branch、PR、工作樹）請使用 `.claude/skills/code-review`；其 Standards 軸的規範來源為上述兩個 coding-style skill 的 `references/`（`backend-coding-style/references/codingStyle.md`、`vue-coding-style/references/vueCodingStyle.md`）。
- 文件中標示「現況落差」的項目，代表既有程式尚未符合規範：新程式依規範撰寫，既有程式只在被修改到該區域時順手收斂，不主動大範圍重構。
- 不得將真實密碼、金鑰、連線字串提交進 repo；本機設定放 `appsettings.Development.json`，範例用 `appsettings.Development.example.json` 佔位。

## Agent skills

### Issue tracker

Issues and specs live as local markdown files under `.scratch/`. See `docs/agents/issue-tracker.md`; when work is finished or deferred, update the spec and ticket statuses as described in its "On completion" section.

### Domain docs

Multi-context: 見 `GLOSSARY-MAP.md` 與 `docs/agents/domain.md`。

## 程式碼導覽

開始實作前先讀 `docs/agents/codebase-map.md`（專案職責、新增簽核單據模組的檢查清單、測試輔助），再依需要讀程式碼。

## 編譯與測試

agent 不編譯、不跑測試，也不在專案資料夾內 restore 或 build；由使用者在本機執行並回報結果。分工與指令見 `docs/agents/build-and-test.md`。

## Git

- 允許在 feature 分支 commit，不在 `main` commit。commit 前先向使用者請求專案資料夾的刪除權限，否則 git 會留下 `.lock` 檔。
- `code-review` 需要至少一個 commit 才有差異可比對：實作完成後先 commit，再審查。
- 不執行 `git push`（需要使用者的 git 憑證），由使用者在 Visual Studio 完成。
