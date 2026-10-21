# 03: 供應商管理前端：列表頁與編輯頁

**What to build:** 在前端提供供應商管理的列表頁與編輯頁（新增、編輯、停用與啟用、刪除、篩選），新增路由與選單入口，比照 `CompanyList.vue`、`CompanyEdit.vue` 與 `companyApi.js`，並遵守 `vue-coding-style`。專案目前沒有進貨單與庫存頁面，進貨單前端串接不在本 ticket。

**Blocked by:** 01

**Status:** ready-for-agent

- [ ] `api/supplierApi.js`：列表、詳情、新增、編輯、停用、啟用、刪除；HTTP 一律走 `api/http.js`，處理 `ResultModel`
- [ ] 供應商列表頁：顯示編號、名稱、統編、負責人、聯絡電話、啟用狀態；關鍵字與啟用狀態篩選；分頁與排序沿用既有慣例；停用與刪除前確認
- [ ] 供應商編輯頁：新增與編輯，欄位含付款天數（整數）與信用額度；前端先做必填與長度、統編 8 碼數字檢查，伺服器端錯誤訊息明確顯示（編號或統編重複、被引用不可刪）
- [ ] 路由與 `DashboardLayout.vue` 選單入口
- [ ] 以 `bash scripts/agent/agent-vue-build.sh` 驗證能建置；在 Comments 記錄未在瀏覽器實際操作的項目

## Comments
