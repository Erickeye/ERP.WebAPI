# 05: 供應商、公司、客戶的刪除引用保護

**Spec:** `.scratch/contract-management/spec.md`

**What to build:** 被合約單引用過的供應商與公司只能停用、不能刪除；被合約單引用過的客戶不能刪除。這是修改既有的刪除檢查，不新增客戶停用功能，也不補其他單據對客戶的引用檢查。

**Blocked by:** 01

**Status:** resolved

- [x] 供應商刪除的引用檢查把合約單算進去，被引用時回明確訊息並建議停用
- [x] 公司刪除的引用檢查把合約單算進去，被引用時回明確訊息並建議停用
- [x] 客戶刪除新增「被合約單引用就擋下」一項檢查，其他行為不變
- [x] 供應商刪除既有規則（被進貨單或庫存項目引用不可刪、報價連帶刪除）不變
- [x] 延伸供應商服務測試與公司服務測試覆蓋新規則；客戶的測試放進合約單服務測試，不新增客戶測試檔
- [x] 若現有測試因規則改變需要調整，已調整並在 Comments 記錄
- [x] 專案編譯與全部測試通過

## Comments

### 完成記錄（05）

- 驗證：`dotnet test src/ERP.Tests/ERP.Tests.csproj` 全套 639 筆通過。編譯輸出仍以 `-p:OutDir=<暫存資料夾>` 導出，原因同 ticket 01。
- 供應商：`IsReferencedAsync` 加入合約單，錯誤訊息改為「供應商已被進貨單、庫存項目或合約單引用，無法刪除，請改用停用」；既有測試只斷言含「停用」，不需調整。新增 `Delete_SupplierReferencedByContract_IsRejectedWithHintToDisable`（同時驗證沒被引用的另一間供應商仍可刪）。
- 公司：`IsReferencedAsync` 加入合約單，訊息「公司已被單據引用，無法刪除，請改用停用」本來就涵蓋；新增 `Delete_ACompanyUsedByAContract_IsRejectedButCanBeDisabled`。
- 客戶：`Delete` 在移除前檢查是否有合約單引用，被引用回 `InvalidRequest`「客戶已被合約單引用，無法刪除」；其他行為（找不到回找不到資料、無引用可刪）不變。不新增停用功能，也沒有補出差單等其他單據的引用檢查（出差單的客戶外鍵仍由資料庫外鍵擋，尚未有友善訊息，屬另案）。測試放在 `ContractServiceTests`：被引用不能刪、沒被引用可刪、只被已作廢合約單引用仍不能刪（已作廢單仍保留對客戶的外鍵）。
- 沒有在真實資料庫驗證外鍵限制刪除的行為（測試的 SQLite 關閉外鍵）。留給 ticket 07。

### code-review 結果（05）

- Spec 軸：0 項錯誤或缺漏。客戶只被已作廢合約單引用時仍不能刪除（作廢單仍保留對客戶的外鍵）與規格一致；供應商既有規則（進貨單與庫存引用、報價連帶刪除）未被破壞；`GetReadableScopeAsync` 重構保持列表、單筆、下載行為不變。風險：外鍵限制刪除未在 SQL Server 驗證；客戶被出差單等其他單據引用時仍會丟資料庫外鍵例外而非友善訊息（規格明確排除）。
- Standards 軸：無硬違規。已處理：客戶測試結尾無意義的 `Assert.NotNull(no)` 改為確認客戶仍存在。未處理的判斷：`ReadableScope` 巢狀類別的位置、commit 標題只寫 ticket 05 但同時帶入 ticket 04 的 spec 回寫與重構。
