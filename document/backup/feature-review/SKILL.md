---
name: feature-review
description: 依據 ERP.WebAPI 的 document/reviewPrompt.md 執行 Code Review。當使用者要求 review 功能、commit、branch、PR 或工作樹時使用。
---

# Feature Review

直接讀取並遵循 `document/reviewPrompt.md`。

不要在本 skill 內重複、擴充或另建 review 規則與輸出格式；有出入一律以該檔案為準。

## 執行原則

1. 先確認 Review 範圍（功能名稱、commit、branch、PR 或工作樹）。使用者沒指定時，預設檢視目前工作樹的變更（`git status`、`git diff`）。
2. 依 `reviewPrompt.md` 的「Review 讀取」段，先讀對應的風格文件：涉及後端讀 `document/codingStyle.md`，涉及前端讀 `document/vueCodingStyle.md`。
3. 嚴重度與輸出格式完全沿用 `reviewPrompt.md`。
4. 只記錄問題與建議，不要未經確認就直接修改程式碼。
5. 文件中標示「現況落差」的既有程式不當作本次 Review 的缺陷，除非本次變更觸及或擴大了該落差。
