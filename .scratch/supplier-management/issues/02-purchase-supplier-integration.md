# 02: 進貨單串接：選單、存在檢查與送簽時停用檢查

**What to build:** 讓進貨單只能選啟用中的供應商，並在儲存與送簽時驗證供應商。填單用選單搬到供應商專屬端點並加權限；`SharedController.GetSupplierSelect` 暫時保留但只回傳啟用中；進貨單儲存時檢查供應商存在；送簽時檢查供應商是否啟用中。已送簽與已核准的進貨單不受停用影響。

**Blocked by:** 01

**Status:** resolved

- [x] **先確認進貨單「送簽」的實際入口與檢查位置**（進貨單走舊簽核流程：`IsApproval`、`PurchaseApprovalTargetHandler`）；若沒有明確的送簽動作，先回頭更新 spec 再繼續，並在 Comments 記錄決定
- [ ] 供應商專屬端點提供填單用選單：只回傳啟用中；可指定一個供應商 Id，使已停用的該供應商一併回傳並標示已停用；選項同時帶出編號與名稱；只要求「進貨單編輯」或「庫存編輯」其中之一，不要求供應商檢視
- [x] `SharedController.GetSupplierSelect` 保留，改為只回傳啟用中的供應商
- [x] 進貨單新增與編輯儲存時，供應商不存在被擋下並有明確訊息（不改其他進貨單驗證）
- [x] 進貨單送簽時，供應商已停用被擋下並提示改選；草稿與已退回的進貨單仍可編輯並顯示原停用供應商
- [x] 已送簽或已核准的進貨單，供應商後來被停用，簽核與核准照常，且照常增加該供應商的庫存項目數量
- [x] 延伸進貨單服務測試，覆蓋存在檢查、送簽停用檢查、已送簽與已核准不受停用影響；供應商服務測試補選單案例
- [x] 供應商服務的引用判斷（ticket 01）與進貨單、庫存項目一致，並以種子資料驗證

## Comments

### 完成記錄（2026-10-21）

實作於 `feat/supplier-management`。後端 `ERP.Tests` 全套 422 項通過（本 ticket 新增：`SupplierServiceTests` 6 項選單測試、`PurchaseServiceTests` 8 項供應商串接測試；SQLite 記憶體庫，不等於 SQL Server）。

送簽入口確認：進貨單的「送簽」就是 `_4010PurchaseService.SendApproval`（`vm.TableId` 為進貨單號），停用檢查加在呼叫簽核引擎之前；找不到進貨單時不擋，交給引擎回報。進貨單沒有狀態欄位（只有 `IsApproval`），所以「草稿、已退回可編輯」沿用既有行為，沒有新增狀態判斷。核准流程（`Approval`）不檢查供應商是否停用。

未勾選項與原因：

- **填單用選單的授權（進貨單編輯或庫存編輯）**：`_4060SupplierController.GetSelect` 已加 `[PermissionAuthorize]`，服務層邏輯（只列啟用中、includeId、編號加名稱）有測試；授權屬控制器層，只經過編譯與靜態檢查，沒有實際呼叫端點驗證。

偏離 ticket／spec 的決定：

- 供應商不存在的錯誤碼用 `FieldValueIsInvalid`（訊息「供應商不存在」），停用時送簽用 `InvalidRequest`。
- 選單文字格式為「編號 名稱」（例如「S001 甲供應商」），沒有編號的舊資料只顯示名稱；排序依編號再依 Id。
- `SharedController.GetSupplierSelect` 保留，改為只回傳啟用中；新程式請改用供應商專屬端點。
- 共用種子資料現在會真的建立一間供應商（原本 `SupplierId` 寫死為 1，沒有對應資料列）。

### code-review 後續修正（2026-10-21）

- `SharedController.GetSupplierSelect` 補上 `[PermissionAuthorize]`（進貨單編輯或庫存編輯），與專屬端點同一組權限，讓 Story 28 對兩個端點都成立。此為行為變更：只有沒有這兩項權限的登入者會被擋；前端目前沒有任何頁面呼叫此端點，若有其他外部呼叫端，需確認其角色具備該權限。
