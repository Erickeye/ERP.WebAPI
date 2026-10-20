---
name: vue-coding-style
description: Apply the ERP.Vue frontend conventions when creating, modifying, reviewing, or refactoring .vue, frontend JavaScript, API wrapper, router, layout, shared component, Element Plus usage, or frontend CSS under src/ERP.Vue. Use it for Vue UI, API-boundary, ResultModel handling, forms, loading/error, confirmation, export/download, pagination, and maintainability work.
---

# Vue Coding Style

依本 skill 資料夾內的 `references/vueCodingStyle.md` 維護 `src/ERP.Vue`。該文件是唯一規範來源；本 skill 只固定執行順序與完成前檢查，不複製整份規範。

## 必做流程

1. 先讀 `references/vueCodingStyle.md`，再讀受影響的 view、`src/api/*`、`src/api/http.js`、`router/index.js`、`DashboardLayout.vue` 與相關工具。
2. 先確認後端 Controller 的實際 Route 與 `SearchVM`／輸入 VM 欄位（Route 為 `/{ModuleNo}{Name}/{Action}`，例如 `/_4010Purchase/Index`），不要先建立平行功能或猜測欄位。
3. 新檔使用 `<script setup>`；順序為 `<script setup>`、`<template>`、`<style scoped>`。UI 優先使用 Element Plus（`el-table`、`el-form`、`el-dialog`、`el-pagination`、`el-date-picker`），不用原生元件重做。
4. HTTP 路徑只放 `src/api`；只有 `http.js` 可 import `axios`。Token 由 request interceptor 帶入，不在每個 wrapper 手動傳 headers；baseURL 取自 `VITE_API_BASE_URL`。
5. 後端回傳是 `ResultModel<T>`，業務失敗也是 HTTP 200：每次呼叫都要檢查 `isSuccess`，不能只靠 `catch`。response 進入畫面前先 `unwrap`、`items.map(normalizeRow)`；template 不寫 `response.data.data.xxx`。
6. 表單用 `el-form` + `rules` + `validate()`；送出前由 `buildPayload()` 處理 trim、轉型、日期、空值與 id；成功才關閉 dialog 與 reload。
7. 每個非同步流程使用獨立 loading state（`loading`／`saving`／`deleting`／`exporting`），`finally` 恢復；錯誤要在畫面可見（頁面或 dialog 內文字加 `ElMessage.error`），後端訊息一律經 `getApiErrorMessage()`。
8. 刪除、停用、簽核、撤銷簽核等高風險操作使用 `ElMessageBox.confirm`；禁止 `window.alert`／`window.confirm`。
9. 下載使用 blob wrapper 與 `utils/file.js`，下載前先判斷是否為 `ResultModel` 失敗回應。
10. 修改 route 或功能時同步檢查 `router/index.js`、導覽選單與登入檢查；Token 讀寫走 `utils/auth.js`，不在 `.vue` 直接碰 `localStorage`。
11. 相同流程出現第二次再抽共用；兩頁規則不同就只共用穩定的 UI、API helper 或下載流程，不建巨型 generic page。
12. CSS 使用 `<style scoped>` 與語意化 class；覆蓋 Element Plus 內部樣式用 `:deep()` 並限縮在頁面根 class 下。

## 完成前檢查

- 以 `rg` 確認 `.vue` 沒有直接 `import axios`、沒有 `window.confirm`／`window.alert`、沒有殘留 `console.log`、沒有寫死 `localhost` 位址。
- 確認每個 API 呼叫都有 `isSuccess` 檢查、loading 與錯誤處理。
- 搜尋重複的日期格式化、下載、確認、分頁流程，能共用就先共用。
- 執行 `git diff --check`；對變更的純 JS 檔可用 `node --check`。
- 除非使用者明確要求，不執行 `npm run build`；交付時說明「僅完成靜態檢查」。

## 例外處理

`references/vueCodingStyle.md` 標示「現況落差」的既有程式（Options API、原生表單元件、寫死 baseURL、逐一傳 headers），只在被修改到該區域時順手收斂，新程式不得複製該寫法。若需動到 `http.js`、router、Layout 或全域 CSS，先向使用者確認範圍。
