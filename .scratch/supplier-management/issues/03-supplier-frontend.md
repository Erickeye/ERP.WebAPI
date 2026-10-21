# 03: 供應商管理前端：列表頁與編輯頁

**What to build:** 在前端提供供應商管理的列表頁與編輯頁（新增、編輯、停用與啟用、刪除、篩選），新增路由與選單入口，比照 `CompanyList.vue`、`CompanyEdit.vue` 與 `companyApi.js`，並遵守 `vue-coding-style`。專案目前沒有進貨單與庫存頁面，進貨單前端串接不在本 ticket。

**Blocked by:** 01

**Status:** resolved

- [x] `api/supplierApi.js`：列表、詳情、新增、編輯、停用、啟用、刪除；HTTP 一律走 `api/http.js`，處理 `ResultModel`
- [x] 供應商列表頁：顯示編號、名稱、統編、負責人、聯絡電話、啟用狀態；關鍵字與啟用狀態篩選；分頁與排序沿用既有慣例；停用與刪除前確認
- [x] 供應商編輯頁：新增與編輯，欄位含付款天數（整數）與信用額度；前端先做必填與長度、統編 8 碼數字檢查，伺服器端錯誤訊息明確顯示（編號或統編重複、被引用不可刪）
- [x] 路由與 `DashboardLayout.vue` 選單入口
- [x] 以 `bash scripts/agent/agent-vue-build.sh` 驗證能建置；在 Comments 記錄未在瀏覽器實際操作的項目

## Comments

### 完成記錄（2026-10-21）

實作於 `feat/supplier-management`。前端用 `scripts/agent/agent-vue-build.sh` 驗證能建置（vite build 通過，未在瀏覽器實際操作）；靜態檢查沒有直接 import axios、`window.confirm`／`alert`、`console.log` 或寫死位址。

新增檔案：`api/supplierApi.js`、`views/SupplierList.vue`、`views/SupplierEdit.vue`，並在 `router/index.js` 加入三條路由（`supplier-list`、`supplier-create`、`supplier-edit`），在 `DashboardLayout.vue` 的「模組設定」卡片加入「供應商管理」入口。畫面結構與公司管理一致。

未在瀏覽器驗證的項目（勾選僅代表建置通過與程式碼審視）：列表篩選與排序、新增／編輯／停用／啟用／刪除的實際操作、後端錯誤訊息（編號或統編重複、被引用不可刪）在畫面上的顯示、付款天數與信用額度輸入元件的行為。

說明與偏離：

- 前端沒有權限模型，按鈕不依權限隱藏；沒有權限時由後端回「權限不足」並顯示該訊息（與公司管理一致）。
- 選單入口放在「模組設定」卡片，與公司管理並列，因為目前沒有庫存管理卡片；日後有庫存模組選單時可移動。
- 付款天數與信用額度用數字輸入元件（0 到 365、0 到 9,999,999,999.99，與後端驗證一致）。
- `supplierApi.getSelect` 已封裝但沒有畫面使用，留給之後的進貨單頁面。
