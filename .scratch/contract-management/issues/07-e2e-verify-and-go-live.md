# 07: 端到端驗證與上線

**Spec:** `.scratch/contract-management/spec.md`

**What to build:** 在真實 SQL Server 與 SFTP 環境完成自動測試無法覆蓋的驗證，建立簽核設定，確認合約單可以從建立走到核准與作廢，並收尾文件與狀態。

**Blocked by:** 05、06

**Status:** deferred

- [ ] 部署腳本在真實資料庫可執行，重複執行也安全
- [ ] 外鍵限制刪除、單號唯一索引在真實資料庫行為正確
- [ ] 列表查詢（篩選、排序、分頁、生效中／已到期）在真實資料庫結果正確
- [ ] 以現有 API 建立一組啟用的合約單簽核設定（指定人員與指定角色各驗證一次）
- [ ] 供應商合約與客戶合約各走一遍：建立草稿、上傳 PDF、送簽、核准、下載 PDF、作廢
- [ ] 拒絕、撤銷、重送各驗證一次
- [ ] 被合約單引用的供應商、公司、客戶無法刪除
- [ ] 沒有簽核設定與重複設定時，送簽的提示正確
- [x] 詞彙表與 spec 無遺漏或過時的敘述；若出現新術語已更新
- [x] 所有 ticket 皆為 resolved 或 deferred 後，把 spec 的 Status 設為 resolved 並註明 deferred 的項目

## Comments

### 狀態記錄（07，deferred）

- 決定者與原因：由實作的 agent 決定延後。這張 ticket 的驗收條件都需要真實環境——SQL Server 資料庫、SFTP 主機、啟用的簽核設定與瀏覽器操作。agent 在使用者本機沒有可用的 ERP 資料庫連線（工具中的資料庫連線屬於其他系統，沒有使用）、沒有 SFTP 帳號，也沒有在瀏覽器實際操作過畫面，所以不能宣稱已驗證。
- 已做（勾選項）：詞彙表（`docs/contract/GLOSSARY.md`、`GLOSSARY-MAP.md`）與 spec 已對照實作更新——spec 的「權限與可見範圍」已回寫 ticket 04 的實作補充，沒有出現未入詞彙表的新術語。
- 全部自動驗證結果：後端 `dotnet test` 全套 639 筆通過（SQLite 記憶體庫）；前端 `npm run build` 通過；`git diff --check` 通過。方案整體 `dotnet build` 只有 `ERP.DB`（SSDT 資料庫專案）無法在 CLI 編譯（缺 Visual Studio 的 SSDT targets），其餘專案編譯通過——**`src/ERP.DB/dbo/Tables/t_3050Contract.sql` 的語法沒有被編譯驗證過**，請在 Visual Studio 建置或發佈 `ERP.DB`。
- 在哪裡接續（使用者在真實環境依序做）：
  1. 在 Visual Studio 建置 `ERP.DB`；既有資料庫執行 `docs/sql/合約單資料表.sql`，執行兩次確認不會重複建立；確認 CHECK（日期、金額、類型與對象）、外鍵、`IX_t_3050Contract_No` 唯一索引都建立。
  2. 以現有簽核設定 API 建立一組啟用的「合約單」（`TableType = 11`）簽核設定，指定人員與指定角色各驗證一次；再各驗證沒有啟用設定與多組啟用設定時送簽的提示。
  3. 設定 SFTP（`SftpConfig`）並實際上傳、更換、下載一份 PDF，確認遠端出現 `/contract-{單號}-{亂數}.pdf`、更換後舊檔被刪除。
  4. 角色權限：給測試角色「合約單檢視／編輯／刪除」；簽核人需要「合約單檢視」才進得了頁面（見 ticket 06 說明）。
  5. 瀏覽器走完：供應商合約與客戶合約各建立草稿 → 上傳 PDF → 送簽 → 核准 → 下載 → 作廢；另走拒絕、撤銷、重送；確認列表篩選（關鍵字、類型、對象、簽約公司、狀態、生效中／已到期、日期區間、待我簽核）、排序（含合約總金額 decimal 排序，SQLite 不支援）與分頁。
  6. 確認被合約單引用的供應商、公司、客戶無法刪除（外鍵限制刪除與友善訊息）。
  7. 確認 `GetApprovalNotify`（引擎「待簽核」查詢）沒有分頁或數量上限，否則「待我簽核」與 `CanApprove` 會漏單。
  8. 全部確認後，把這張 ticket 的勾選與 Status 補完，並把 spec 的 Status 維持 resolved（見下）。
- spec 的 Status 已依規則設為 resolved 並註明本 ticket deferred；若真實環境驗證發現問題，請重開對應 ticket。
