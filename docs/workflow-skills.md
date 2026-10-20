# 工作流程 Skills 介紹

本專案除了 `backend-coding-style`、`vue-coding-style`（規範類，規定「程式該怎麼寫」）之外，另外引入一組**工作流程類** skill，規定「怎麼跟 AI 協作」：先對齊需求、再寫規格、拆 ticket、測試先行實作、審查、除錯、事後檢討、跨 session 交接。

- 來源：GitHub `mattpocock/skills`（Matt Pocock）
- 原則：**原文照搬，不修改 skill 內容**。要調整專案相關設定，一律改 `AGENTS.md` 或 `docs/agents/`，不改 skill 本身。
- 位置：`.claude/skills/`

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
一輪做完之後    →  /retro（檢討這次 agent 環境哪裡卡住，建議怎麼改善）
中途想換 session →  /handoff（把對話壓縮成交接文件給下一個 agent）
```

`to-tickets` 是選用步驟：小到中等的功能可以從 `to-spec` 直接進 `implement`，大功能或要拆給多個 session 時再拆 ticket。

---

## 使用者主動呼叫的 skill（輸入 `/名稱`）

| Skill | 用途 | 何時用 |
|---|---|---|
| `grill-with-docs` | 被 AI 逐輪「拷問」需求，每題附建議答案，直到所有決策都清楚；同時更新 `GLOSSARY.md` 與 ADR | 任何新功能、改動開工前，**建議每次都先用** |
| `to-spec` | 不再提問，直接把目前對話整理成規格並發佈到 issue tracker | grill 完、要進入多 session 開發時 |
| `to-tickets` | 把規格拆成 tracer-bullet ticket，標明誰擋誰 | 規格確認後 |
| `implement` | 依規格或 ticket 實作，自動走 tdd 與 code-review，最後 commit | 開始寫 code 時 |
| `improve-codebase-architecture` | 掃描程式碼找「可加深的模組」，產出 HTML 報告，再挑一項討論 | 有空時定期跑，是維護性工作 |
| `retro` | 對一次 coding session 做檢討，依嚴重度建議如何改善 agent 的**環境**（導覽、自動檢查、規範、steering 檔、工具、資訊取得） | 一輪流程做完、或某次 session 卡很久之後 |
| `handoff` | 把目前對話壓縮成交接文件（存到系統暫存目錄），讓全新的 agent 接續工作；已記錄在 spec、ADR、commit 的內容只放路徑，不重複；會遮蔽敏感資訊 | 對話太長、要換 session 前 |
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
| `writing-for-agents` | 撰寫給 agent 讀的文件（skill、`AGENTS.md`）的原則；`retro` 第一步會呼叫它，因此一併安裝 |

---

## 專案內的搭配規則

- **規範來源**：`code-review` 的 Standards 軸，以兩個 coding-style skill 的 `references/`（`codingStyle.md`、`vueCodingStyle.md`）為準（已寫在 `AGENTS.md`）。
- **首次使用**：先執行 `/setup-matt-pocock-skills`，設定 issue tracker 後，`to-spec`、`to-tickets` 才能正常運作。

## 注意事項

- 這組 skill 的重點是「少而深的模組」。`improve-codebase-architecture` 與 `codebase-design` 的建議，可能與本專案 Controller → Service 的分層規範不同，僅供參考，是否採用由人決定。
- 部分範例以 TypeScript 撰寫，概念通用，實際以 C# / .NET 對應。
- 來源 repo 的 skill 分在 `skills/engineering/` 與 `skills/productivity/` 兩個資料夾：`retro` 在 engineering，`handoff` 與 `writing-for-agents` 在 productivity。
- 更新方式：重新從來源 repo 複製對應資料夾（Claude Code 不需要其中的 `agents/openai.yaml`，已移除）。
- 在 Codex 使用時，skill 需放在 `.agents/skills/`，不是 `.claude/skills/`。

---

## 實際跑完一輪（領料單功能）的經驗

2026-10 以「領料單」功能完整跑過 grill-with-docs → to-spec → implement → code-review。實測發現：

- **使用者呼叫型 skill 不能由 AI 自己啟動。** `implement`、`retro`、`handoff` 等標了 `disable-model-invocation: true`，AI 無法用 Skill 工具呼叫，必須由使用者輸入。
- **`code-review` 在本機被 `skillOverrides` 停用了對模型的呼叫**（`code-review` 的 SKILL.md 本身沒有停用標記，來源是設定檔）。結果 `implement` 做到最後一步無法自動呼叫它，審查要由使用者另外觸發。要讓 `implement` 一次跑完 coding → tdd → review → commit，需先移除這個覆寫；移除前先確認當初為何停用（審查會開兩個子 agent，用量不小）。
- **Claude 桌面 App（Cowork）輸入 `/code-review` 會顯示 Unknown skill**，專案內的 skill 要在實際支援的工具（例如 Claude Code）才能用斜線指令呼叫。
- **`tdd` 要求先與使用者確認測試的 seam，再一次一個垂直切片做紅綠燈。** 第一輪是先寫完程式再補測試，不符合這個流程，下一個功能應照流程做。
- **`to-tickets` 第一輪被略過**，直接由 spec 實作。
- **`retro` 提到的 `CODING_STANDARDS.md`，本專案對應的是 `backend-coding-style/references/codingStyle.md`**，不是同名檔。
- **規格與草稿放在 `.scratch/`**（由 `docs/agents/issue-tracker.md` 設定），是會結案的工作單據；`docs/<context>/adr/` 與 `docs/<context>/GLOSSARY.md` 是長期保留的參考文件。
- **在 agent 的工作階段內編譯與測試**，請照 `AGENTS.md`「編譯與測試（agent 工作階段）」的離線還原方式；commit 與 push 由使用者在 Visual Studio 完成。
- **agent 無法寫入 `.claude/` 資料夾**（遠端工具會拒絕），新增或更新 skill 需由使用者自己複製。
