# 03: 供應商報價前端：報價管理頁與入口

**What to build:** 有權限的人能在供應商列表點「報價」進入某供應商的報價管理頁，檢視、搜尋、新增、編輯與刪除該供應商的報價。比照現有供應商頁面與 API 封裝，遵守專案前端規範。進貨單與庫存頁面專案目前沒有，填單查詢的前端串接延後。

**Blocked by:** 01

**Status:** resolved

- [ ] 供應商列表每列有「報價」入口，點擊進入該供應商的報價管理頁（路由帶供應商 Id），入口依供應商檢視權限顯示
- [ ] 報價管理頁顯示料號、品名、單位、單價、備註、最後修改時間；可用關鍵字篩選；預設依料號排序；分頁
- [ ] 新增與編輯用對話框，必填與長度、單價大於 0 的規則與後端一致，後端錯誤訊息明確顯示
- [ ] 刪除需確認，且按鈕只對有供應商刪除權限的人顯示；新增與編輯按鈕只對有供應商編輯權限的人顯示
- [ ] 頁面標示所屬供應商（編號與名稱），供應商不存在時有明確提示
- [x] 以 `npm run build` 驗證能建置；在 Comments 記錄未在瀏覽器實際操作的項目

## Comments

### 完成記錄（2026-10-21）

- 新增 `api/supplierPriceApi.js`、`views/SupplierPriceList.vue`、`utils/permissions.js`；修改 `router/index.js`、`utils/auth.js`、`views/SupplierList.vue`。路由為 `/supplier/:supplierId/price`（name `supplier-price-list`），不新增側邊選單項目，入口在供應商列表每列的「報價」。
- 驗證：`npm run build` 通過（僅有既有的 chunk 過大警告）；靜態檢查 `.vue` 與新檔沒有直接 import axios、`window.confirm`／`alert`、`console.log`、寫死位址；`git diff --check` 通過。
- **未在瀏覽器實際操作**：沒有啟動前後端做畫面操作，也未連真實後端與不同權限帳號測試，因此前五項驗收（入口與權限顯示、列表／篩選／排序／分頁、對話框驗證與後端錯誤顯示、刪除確認與按鈕權限、供應商不存在提示）只有程式碼與建置層級的確認，保持不勾，待手動驗證。
- 偏離規格或補充的決定：
  - 既有前端沒有任何權限判斷機制（供應商頁面全靠後端限制），因此新增 `getPermissions()`／`hasPermission()`（讀 `utils/auth.js` 取得的 JWT `permissions` claim，內容是整數陣列）與 `utils/permissions.js` 常數（供應商檢視 4060、編輯 40600、刪除 40601，對應後端 `PermissionType`）。
  - 路由加 `meta.permission`，`router.beforeEach` 對有設定的路由檢查權限，無權限導回 `layout`；其他既有路由不受影響。前端控制只是 UX，真正限制仍在後端。
  - 報價的最後修改時間欄與單價欄可排序（custom sort，欄位名稱為 VM 屬性）；單價顯示固定兩位小數。
