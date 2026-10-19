# 工作流程 Skills 介紹

本專案除了 `backend-coding-style`、`vue-coding-style`（規範類，規定「程式該怎麼寫」）之外，另外引入一組**工作流程類** skill，規定「怎麼跟 AI 協作」：先對齊需求、再寫規格、拆 ticket、測試先行實作、審查、除錯。

- 來源：GitHub `mattpocock/skills`（Matt Pocock）
- 原則：**原文照搬，不修改 skill 內容**。要調整專案相關設定，一律改 `AGENTS.md` 或 `docs/agents/`，不改 skill 本身。
- 位置：`.claude/skills/`
- 舊的 `feature-review` skill 與 `reviewPrompt.md` 已封存於 `document/backup/`，不再使用。

---

## 主流程

```
想法 / 需求
  │
  ▼
/grill-with-docs      逐輪提問，釐清需求，並沉澱術語表與決策紀錄
  │
  ▼
/to-spec              把討論結果整理成規格
  │
  ▼
/to-tickets           拆成有相依關係的 ticket（每張都是可驗證的垂直切片）
  │
  ▼
/implement            實作（內部自動使用 tdd，完成後自動 code-review）

遇到 bug        →  /diagnosing-bugs
定期檢視架構    →  /improve-codebase-architecture
```

流程中途想換 session，可用 `/handoff`（本專案未安裝，需要時再加）。

---

## 使用者主動呼叫的 skill（輸入 `/名稱`）

| Skill | 用途 | 何時用 |
|---|---|---|
| `grill-with-docs` | 被 AI 逐輪「拷問」需求，每題附建議答案，直到所有決策都清楚；同時更新 `GLOSSARY.md` 與 ADR | 任何新功能、改動開工前，**建議每次都先用** |
| `to-spec` | 不再提問，直接把目前對話整理成規格並發佈到 issue tracker | grill 完、要進入多 session 開發時 |
| `to-tickets` | 把規格拆成 tracer-bullet ticket，標明誰擋誰 | 規格確認後 |
| `implement` | 依規格或 ticket 實作，自動走 tdd 與 code-review，最後 commit | 開始寫 code 時 |
| `improve-codebase-architecture` | 掃描程式碼找「可加深的模組」，產出 HTML 報告，再挑一項討論 | 有空時定期跑，是維護性工作 |
| `setup-matt-pocock-skills` | **一次性**設定：issue tracker、領域文件位置 | 第一次使用前，每個 repo 只跑一次 |

## AI 會自動使用的 skill（也可手動呼叫）

| Skill | 用途 |
|---|---|
| `tdd` | 測試先行：紅燈 → 綠燈，一次一個垂直切片，並規範好測試與壞測試的樣子 |
| `diagnosing-bugs` | 除錯紀律：沒有「一個指令就能重現這個 bug」之前，禁止先猜原因 |
| `code-review` | 同時從兩個軸審查變更：**Standards**（是否符合規範）與 **Spec**（是否符合需求），兩軸分開呈現 |

## 被其他 skill 呼叫的底層 skill（通常不需直接用）

| Skill | 用途 |
|---|---|
| `grilling` | 提問引擎：把需求視為決策樹，每輪問完所有已可回答的問題 |
| `domain-modeling` | 維護 `GLOSSARY.md` 術語表與 ADR，發現用詞混亂時當場指出 |
| `codebase-design` | 架構詞彙表（模組、介面、深度、接縫），供 tdd 與架構改進使用 |

---

## 專案內的搭配規則

- **規範來源**：`code-review` 的 Standards 軸，以 `document/codingStyle.md`、`document/vueCodingStyle.md` 與兩個 coding-style skill 為準（已寫在 `AGENTS.md`）。
- **封存資料夾**：`document/backup/` 內容不屬於現行規範，所有 skill 一律忽略。
- **首次使用**：先執行 `/setup-matt-pocock-skills`，設定 issue tracker 後，`to-spec`、`to-tickets` 才能正常運作。

## 注意事項

- 這組 skill 的重點是「少而深的模組」。`improve-codebase-architecture` 與 `codebase-design` 的建議，可能與本專案 Controller → Service 的分層規範不同，僅供參考，是否採用由人決定。
- 部分範例以 TypeScript 撰寫，概念通用，實際以 C# / .NET 對應。
- 更新方式：重新從來源 repo 複製對應資料夾（Claude Code 不需要其中的 `agents/openai.yaml`，已移除）。
- 在 Codex 使用時，skill 需放在 `.agents/skills/`，不是 `.claude/skills/`。
