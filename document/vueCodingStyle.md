# ERP.Vue 前端 Coding Style

本文件是 `src/ERP.Vue` 前端修改的共同規範，後端請見 `document/codingStyle.md`。

規範以 ERP.Vue 實際使用的技術為準：**Vue 3、Vue Router、Element Plus、Axios、Vite**（`@` alias 指向 `src`）。目前沒有 Pinia、ESLint、Prettier；需要時再導入，導入時同步更新本文件。若需求需要改變架構，先說明影響範圍，再修改本文件與程式碼。

> 標示「**現況落差**」的項目代表既有程式尚未符合規範：新程式依規範撰寫；既有檔案在被修改到該區域時順手收斂，不需為此單獨大改。

## 1. 基本原則

- 新檔案使用 Vue 3 Composition API 與 `<script setup>`。
- 頁面只負責畫面狀態、使用者互動與資料轉換；HTTP 細節集中在 `src/api`。
- API 回應在進入畫面前完成 unwrap 與欄位正規化，template 只使用前端欄位。
- UI 元件優先使用 **Element Plus**（`el-table`、`el-form`、`el-dialog`、`el-pagination`、`el-date-picker`、`el-select`、`ElMessage`、`ElMessageBox`），不要用原生 `<table>`／`<input type="date">` 重做同樣功能。
- 相同 UX 或資料流程出現第二次，先找出可共用的邊界（共用元件／composable／util），不要複製一份狀態、錯誤處理與 CSS。
- 不為了抽象而抽象；兩頁商業欄位或儲存規則不同，只共用穩定的 UI、API helper 或下載流程。
- 不在 template 堆複雜轉換、日期格式化或 API 判斷；這些放 script 的 function、computed 或 `src/utils`。
- 商業規則以後端為準，前端只做輸入整理、顯示與基本驗證。

## 2. 目前前端結構與檔案命名

| 類型 | 路徑 | 命名與責任 |
| --- | --- | --- |
| 路由頁面 | `src/views` | `{Feature}List.vue`、`{Feature}Create.vue` 等，組合頁面流程；模組變多時依模組分資料夾 |
| 共用元件 | `src/components/common` | `App{Name}.vue`（如 `AppPagination`）；既有 `Notification.vue` 保持原名 |
| Layout 元件 | `src/components` | `DashboardLayout.vue`，只處理 shell、導覽 |
| API wrapper | `src/api` | `{feature}.js`，以具名匯出函式（既有 `dayoff.js`）；新檔建議 `{feature}Api.js` 匯出物件 |
| composable | `src/composables` | `use{Name}.js`，只抽可重用的狀態與行為（目錄需要時再建立） |
| 工具函式 | `src/utils` | 依用途命名（既有 `dateUtils.js`） |
| 全域樣式 | `src/assets` | `base.css`、`main.css` |

匯入 `src` 內檔案使用 `@/` alias；同一檔案不要對同一模組混用相對與 alias 兩種寫法。

## 3. 後端 Contract（前後端對照）

- 後端 Route 為 `api/[controller]/{Action}`，Controller 名稱含模組編號，例如 `/_1030DayOff/Index`、`/_4010Purchase/Add`。API wrapper 的路徑必須與 Controller 實際 Route 一致，不自行改成短路由。
- 列表 Action 統一叫 `Index`，查詢參數對應 `SearchVM`：`PageNumber`、`PageSize`、`SortColumn`、`SortDirection` 加上該功能的查詢欄位。
- 回傳一律是 `ResultModel<T>`（System.Text.Json 預設 camelCase）：

```json
{ "isSuccess": true, "errorCode": 0, "errorMessage": null, "data": {} }
```

- 列表 `data` 形狀為 `{ items, pageNumber, pageSize, totalCount }`；下拉等清單可能是 `{ items }`。
- 驗證失敗時 `data` 為 `{ 欄位名: [錯誤訊息...] }`。
- HTTP 層級的 401（未登入／Token 失效）由框架回應，不是 `ResultModel`；其餘業務錯誤都是 HTTP 200 加 `isSuccess: false`，**所以不能只靠 `catch` 判斷失敗**。

## 4. SFC 結構與撰寫順序

`<script setup>` 順序：Vue imports、Element Plus imports、API imports、共用元件 imports、狀態、computed、正規化／格式化函式、互動函式、生命週期。區塊順序固定為 `<script setup>`、`<template>`、`<style scoped>`。

```vue
<script setup>
import { onMounted, reactive, ref } from 'vue'
import { ElMessage } from 'element-plus'
import { dayOffApi } from '@/api/dayOffApi'
import { unwrap, isSuccess, getApiErrorMessage } from '@/utils/api'
import { formatDate } from '@/utils/dateUtils'

const loading = ref(false)
const pageError = ref('')
const rows = ref([])
const search = reactive({ startDate: '', endDate: '', pageNumber: 1, pageSize: 10 })
const pagination = reactive({ totalCount: 0 })

function normalizeRow(row = {}) {}
function buildQuery() {}
async function loadRows() {}
async function searchRows() {}

onMounted(loadRows)
</script>
```

規則：

- 每個 function 只做一件事，不把 API、判斷與 UI 狀態壓成一行。
- `ref` 用於 primitive、陣列與 loading；`reactive` 用於表單、查詢條件與分頁資訊。
- 陣列常數（欄位、選項）放 script，不在 template 重複建立。
- 相同三元運算式不要複製到多個 slot，改用 computed 或格式化 function。
- `onMounted(loadRows)` 只用於初次載入；依 route 參數重新載入用 `watch`，並處理競速（後送先回）。
- template 事件呼叫具名 function，不在 `@click` 內塞多段邏輯。
- **現況落差**：既有 `DayOffList.vue`、`DayOffCreate.vue`、`LoginPage.vue` 使用 Options API 與原生 HTML 表單元件；改到這些頁面時，一併改為 `<script setup>` 與 Element Plus。

## 5. API 邊界與 wrapper

只有 `src/api/http.js` 可以 import `axios`；其他 `.vue`、composable、util 不得直接使用 Axios。包含登入、檔案下載與上傳，都要有對應的 wrapper。

```js
// src/api/purchaseApi.js
import http from './http'

export const purchaseApi = {
  index(params) {
    return http.get('/_4010Purchase/Index', { params })
  },
  get(id) {
    return http.get('/_4010Purchase/Get', { params: { id } })
  },
  add(data) {
    return http.post('/_4010Purchase/Add', data)
  },
  edit(data) {
    return http.post('/_4010Purchase/Edit', data)
  },
  delete(id) {
    return http.delete('/_4010Purchase/Delete', { params: { id } })
  }
}
```

### 5.1 http.js 要處理的事

`src/api/http.js` 是唯一的 Axios instance，集中處理：

- **baseURL 取自環境變數**（`import.meta.env.VITE_API_BASE_URL`，設定在 `.env.development`／`.env.production`），不寫死。
- **request interceptor** 自動帶入 `Authorization: Bearer {token}`，頁面與 wrapper 不再各自傳 headers。
- **response interceptor** 統一處理 HTTP 401：清除 token、導回登入頁。
- 陣列 query 的序列化格式在此統一，對應後端 model binding。

**現況落差**：`http.js` 目前 baseURL 寫死為 `https://localhost:7129/api`，且 `dayoff.js` 每個函式都手動傳 `getHeaders()`。新增或修改 wrapper 時請順手改為 interceptor 模式，並移除各函式的 `headers: getHeaders()`。

### 5.2 Token 保存

目前以 `localStorage` 的 `jwt` 保存 Token。規則：

- 讀寫 Token 只能透過 `src/utils/auth.js`（集中 `getToken`／`setToken`／`clearToken`），不在各頁面直接碰 `localStorage`。
- 不把密碼、完整個資存入 `localStorage`。
- 登出必須清除 Token 並導回登入頁。

## 6. API 回應、正規化與 payload

共用 helper 集中在 `src/utils/api.js`（需建立），頁面只保留領域專用的 `normalizeRow`：

```js
export function unwrap(response) {
  const result = response?.data ?? response
  return result?.data ?? null
}

export function isSuccess(response) {
  const result = response?.data ?? response
  return result?.isSuccess === true
}

export function getApiErrorMessage(error, fallback = '操作失敗') {
  const result = error?.response?.data ?? error?.data ?? error
  return result?.errorMessage ?? fallback
}
```

> 以上僅示意命名與邊界；純檔案下載等沒有 `isSuccess` 的 response 不可硬套 mutation 的成功判斷。

列表資料流程固定為：

1. API wrapper 取得 response。
2. `isSuccess` 檢查，失敗顯示錯誤並結束。
3. `unwrap` 取出 `data`，讀取 `items`、`totalCount`。
4. `items.map(normalizeRow)` 後才寫入 `rows`。
5. template 只讀取已定義預設值的前端欄位。

送出流程固定為：

1. 表單以 `v-model` 維持畫面狀態。
2. `buildPayload()` 負責 trim、Number、日期、空值與子集合轉換。
3. 編輯時保留 `id`，不要把顯示文字當成後端 id。
4. 呼叫 API 後**明確檢查 `isSuccess`**，成功才關閉 dialog、重新載入列表。

**現況落差**：`DayOffList.vue` 直接寫 `response.data.data.items`，且只用 `catch` 判斷失敗；業務失敗（`isSuccess: false`）不會被看見。

## 7. 搜尋、列表與分頁

```js
const search = reactive({
  keyword: '',
  pageNumber: 1,
  pageSize: 10,
  sortColumn: '',
  sortDirection: 'asc'
})

async function searchRows() {
  search.pageNumber = 1
  await loadRows()
}
```

- 欄位名稱對齊後端 `SearchVM`（日期欄位名稱照 VM 屬性，例如 `PurchaseDateStart`，送出時 axios 會原樣帶出；前端可用 camelCase 後端 binding 不分大小寫）。
- 搜尋按鈕與 Enter 都呼叫同一個 `searchRows()`；條件改變回到第 1 頁。
- 使用 `el-pagination` 的 `v-model:current-page`、`v-model:page-size` 與 `@current-change`／`@size-change` 重新查詢；請求期間 disabled。
- 空字串送出前轉 `undefined`，讓後端 `[SearchField]` 略過該條件，不要把「全部」送成特殊值。
- `el-table` 搭配 `v-loading="loading"`；欄位需要 badge、日期、操作時使用 slot；每列 `row-key` 使用後端 id。
- 查詢、儲存、刪除、匯出使用各自的 loading state（`loading`、`saving`、`deleting`、`exporting`），不要共用一個。
- 排序：`el-table` 的 `@sort-change` 轉成 `sortColumn`／`sortDirection`，欄位值必須是後端 `SortHelper` 允許的 VM 屬性名稱。

## 8. 表單、Dialog 與輸入元件

- 表單使用 `el-form` + `:model` + `:rules` + `ref`，送出前 `await formRef.value.validate()`。
- 字串欄位通常 `v-model.trim`；數字欄位使用 `el-input-number` 或 `.number`；`el-select` 的 option value 型別要一致。
- 日期使用 `el-date-picker`，`value-format="YYYY-MM-DD"`（或日期時間 `YYYY-MM-DDTHH:mm:ss`），讓送出值就是後端可接受的 ISO 字串，不在每頁重寫格式轉換。
- 新增與編輯若流程相同，共用一個 `el-dialog`，以 `dialogMode` 控制；不同商業流程才拆開。
- `openAdd()` 先 reset form；`openEdit(row)` 先呼叫 `get` 取得單筆、normalize 後再填入，不要把列表 row 直接當完整編輯 model。
- 後端驗證失敗（`data` 為欄位錯誤字典）時，把訊息對應顯示到欄位或表單錯誤區，不只顯示「操作失敗」。
- 前端驗證只是第一道防線，不可視為唯一防線。
- 共用下拉資料（人員、假別等）使用後端 `SharedController` 提供的 API，並在 wrapper 層集中，不在各頁各寫一份。

## 9. Loading、錯誤與高風險操作

所有非同步操作都要有明確的開始、成功、失敗與結束狀態：

```js
async function save() {
  saving.value = true
  formError.value = ''

  try {
    const response = await purchaseApi.edit(buildPayload())
    if (!isSuccess(response)) {
      formError.value = getApiErrorMessage(response, '儲存失敗')
      return
    }

    ElMessage.success('儲存成功')
    dialogVisible.value = false
    await loadRows()
  } catch (error) {
    formError.value = getApiErrorMessage(error, '儲存失敗')
  } finally {
    saving.value = false
  }
}
```

規則：

- 按鈕在對應 state 為 true 時 disabled／loading，避免重複送出；`finally` 一定恢復 state。
- 錯誤要放在頁面或 dialog 內可見位置，同時用 `ElMessage.error` 做第一時間提示；兩者並存，不為避免重複而拿掉其中一個。
- 後端錯誤訊息一律經 `getApiErrorMessage()` 取得，不在頁面自行解析 `response.data.errorMessage`。
- 不可 catch 後靜默 return，也不可失敗仍關閉 dialog 或清除目標。
- **不使用 `window.alert`／`window.confirm`**。高風險操作（刪除、停用、覆蓋、送出簽核、撤銷簽核）一律使用 `ElMessageBox.confirm`：

```js
async function deleteRow(row) {
  try {
    await ElMessageBox.confirm(`確定要刪除「${row.no}」？此操作無法復原。`, '刪除資料', {
      confirmButtonText: '刪除',
      cancelButtonText: '取消',
      type: 'warning'
    })
  } catch {
    return // 使用者取消
  }

  deleting.value = true
  try {
    const response = await purchaseApi.delete(row.id)
    if (!isSuccess(response)) {
      ElMessage.error(getApiErrorMessage(response, '刪除失敗'))
      return
    }
    ElMessage.success('已刪除')
    await loadRows()
  } catch (error) {
    ElMessage.error(getApiErrorMessage(error, '刪除失敗'))
  } finally {
    deleting.value = false
  }
}
```

- 「API 成功」與「重新載入成功」兩個階段各自處理錯誤，不要塞進同一個無法追蹤的 expression。
- **現況落差**：既有 `Notification.vue` 是自製提示元件；新程式改用 `ElMessage`，兩者並存期間不要在同一頁混用。

## 10. 檔案上傳、下載與匯出

- 上傳在 wrapper 建立 `FormData`，欄位名稱對照後端 request VM；不手動設定 multipart `Content-Type`，讓瀏覽器建立 boundary。
- 下載（Excel／PDF）使用 `responseType: 'blob'` 的 wrapper，並在 `src/utils/file.js` 集中處理 Blob → 下載、檔名解析（`Content-Disposition`），不在各頁重寫。
- 下載若後端以 `ResultModel` 失敗回應（blob 內容其實是 JSON），要先判斷再顯示錯誤，不可下載出一個壞檔案。
- 匯出／匯入各自維護 `exporting`、`importing` state 與錯誤訊息。
- 匯出按鈕對應後端 `[Log(Export)]` 的 Action，不要繞過後端用前端自行組檔處理敏感資料。

## 11. Router、登入與 Layout

- 新頁面在 `router/index.js` 註冊，並同步加入 Layout 導覽（`DashboardLayout.vue`）；兩處都要確認。
- 需要登入的 route 由 `router.beforeEach` 檢查 Token；Token 讀取走 `src/utils/auth.js`。
- 需要權限的頁面或按鈕：後端 JWT 帶有 `permissions` claim，前端顯示控制只是 UX，**真正限制一律在後端**。
- 既有 route name、path 與 API action 是跨檔案 contract，修改前先搜尋所有引用。
- Layout 只處理導覽與 shell，不放頁面商業查詢。
- 目前沒有 Pinia；若跨頁共用狀態（登入使用者摘要、權限、全域設定）增加到需要集中管理時，再導入 Pinia，且只放跨頁狀態，頁面 rows、form、dialog target 不放 store。

## 12. CSS 與可維護性

- Element Plus 預設樣式與 CSS 變數優先；全站共用樣式放 `src/assets/main.css`。
- 頁面特殊樣式使用 `<style scoped>`；需要覆蓋 Element Plus 內部樣式時使用 `:deep()`，並限制在頁面根 class 之下。
- class 使用語意命名（`search-bar`、`row-actions`），不用 `.red`、`.box2`。
- 不在 template 寫大段 inline style。
- 不在頁面複製共用規則；兩頁需要相同樣式就移到共用 CSS 或元件。

## 13. 設定與機密

- API 位址、環境相關設定放 `.env.*`（以 `VITE_` 開頭才會暴露給前端），`.env.local` 不提交。
- 前端不放任何密鑰、帳密或內部主機位址；瀏覽器端的所有內容都視為公開。

## 14. 靜態檢查（規劃中）

目前沒有 ESLint／Prettier。導入時建議：

- ESLint flat config + `eslint-plugin-vue`，`no-restricted-imports` 禁止在 `http.js` 以外 import `axios`，`no-restricted-globals` 禁止 `alert`／`confirm`。
- Prettier 先只設定檔，不對既有檔案全面重排，避免整檔 diff。
- 完成後在 `package.json` 加 `lint`、`format:check` scripts，並更新本節。

## 15. Frontend Done Definition

前端功能符合以下項目才可標示為「完成」：

1. route 與導覽已接通，需登入的頁面有 Token 檢查。
2. 列表資料由 API 載入，不以 static rows 當正式資料源。
3. 搜尋條件、排序、分頁送到正確的 `SearchVM` 欄位，查詢後列表與筆數更新。
4. API response 已 `isSuccess` 檢查與 unwrap／normalize，template 不處理後端結構差異。
5. 新增、編輯使用 `el-form` 驗證、`buildPayload()`、正確 id；後端欄位錯誤有顯示。
6. 儲存、刪除、簽核、匯出各自有明確 API 與 loading／error state。
7. 高風險操作使用 `ElMessageBox.confirm`；成功後才關閉 dialog、reload 或下載。
8. 沒有複製第二份相同的分頁、日期、確認、下載流程。
9. 失敗訊息在畫面上看得見，沒有 silent catch、失敗仍顯示成功或無法重試。
10. `git diff --check` 通過；有明確要求或風險需要時，再執行 `npm run build` 與實際頁面 smoke test。

## 16. 禁止事項

- 在頁面或 composable 直接使用 Axios，或每個 wrapper 函式各自帶 `Authorization` header。
- 把 baseURL、主機位址、密鑰寫死在程式碼。
- 在 `.vue` 直接讀寫 `localStorage`（Token 一律走 `utils/auth.js`）。
- 把 static rows 當作已完成的 API 功能。
- 只靠 `catch` 判斷失敗而不檢查 `isSuccess`。
- 在 template 散落 `response.data.data.xxx` 之類的深層取值。
- 使用 `window.confirm`、`window.alert`，或 `console.log` 殘留在提交的程式碼中。
- 在第二個頁面複製已存在的 dialog、確認、分頁、日期或下載流程。
- 把商業規則硬編碼在前端；前端權限控制當成安全機制。
- 需求未確認前大幅重構 router、layout、全域 CSS 或 API contract。
- 為符合樣板而套用未使用或 contract 不明的 composable／共用元件。
