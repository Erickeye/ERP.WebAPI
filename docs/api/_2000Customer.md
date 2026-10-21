# 客戶（_2000Customer）API 規格

| 項目 | 內容 |
|---|---|
| 模組 | 客戶 |
| 日期／負責人 | 2026-10-21 ／ randal |
| 需求來源 | 無獨立需求文件，依現有實作整理 |
| 範圍 | 客戶列表、檢視、新增、修改、刪除、下拉選單；客戶聯絡人（Custemploy）不在本文件 |

## 共通約定

- **驗權**：全部 API 需帶 `Authorization: Bearer {token}`；無權杖或無效時回 `1008`。
- **格式**：JSON，欄位為小駝峰；GET 用網址參數，POST 用 JSON 本文（刪除例外）。
- **成敗**：一律 HTTP 200，看 `errorCode`，`0` 為成功。回應外框為 `isSuccess`、`errorCode`、`errorMessage`、`data`。
- **欄位驗證失敗**：回 `4000`，`data` 為「欄位 → 錯誤訊息清單」。
- **發票格式**：`0` 二聯式、`1` 三聯式（預設）、`2` 其他。

## 涉及的操作項目

| 操作 | 說明 | 對應 API | 服務代號／優先序 |
|---|---|---|---|
| 檢視客戶列表 | 取得全部客戶摘要 | `GET /api/_2000Customer/Index` | —／— |
| 檢視客戶資料 | 以流水號取得單一客戶 | `GET /api/_2000Customer/Get` | —／— |
| 新增客戶資料 | 建立客戶 | `POST /api/_2000Customer/Create` | —／— |
| 修改客戶資料 | 更新客戶 | `POST /api/_2000Customer/Edit` | —／— |
| 刪除客戶資料 | 以流水號刪除客戶 | `POST /api/_2000Customer/Delete` | —／— |
| 取得客戶下拉選單 | 取得客戶流水號與名稱 | `GET /api/Shared/GetCustomerSelect` | —／— |

## 各 API 明細

### 【檢視客戶列表】

PATH

```
GET /api/_2000Customer/Index
```

REQ：無

RSP

```json
{
  "isSuccess": true,
  "errorCode": 0,
  "errorMessage": null,
  "data": {
    "items": [
      { "id": 1, "name": "TEST_客戶A", "taxInvoiceNumber": "12345678", "contactPhone": "02-1234-5678", "registeredAddress": "TEST_台北市" }
    ]
  }
}
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 1008 | 無效的權杖 | `InvalidToken` |

- 備註：不分頁、不排序。**現況落差**：目前 `items` 永遠為空陣列。

### 【檢視客戶資料】

PATH

```
GET /api/_2000Customer/Get?id=1
```

REQ

```
id=1    // 必填，客戶流水號（網址參數）
```

RSP

```json
{
  "isSuccess": true,
  "errorCode": 0,
  "errorMessage": null,
  "data": {
    "id": 1,
    "attribName": "TEST_A",
    "name": "TEST_客戶A",
    "taxInvoiceNumber": "12345678",
    "owner": "TEST_王小明",
    "contactPhone": "02-1234-5678",
    "faxPhone": null,
    "staffChineseName": "TEST_業務甲",
    "registeredAddress": "TEST_台北市",
    "deliveryAddress": null,
    "taxInvoiceAddress": null,
    "bankName": null,
    "checkingAccount": null,
    "remittanceAccount": null,
    "payDays": "30",
    "creditLine": 100000,
    "creditBalance": 50000,
    "lastDeliveryDate": "2026-10-01",
    "advance": 0,
    "invoiceForm": 1
  }
}
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 4001 | 找不到該資料 | `NotFoundData` |
| 1008 | 無效的權杖 | `InvalidToken` |

- 備註：**現況落差**：目前不依 `id` 篩選，任何 `id` 都回資料表第一筆（`id` 欄為請求值），僅資料表為空時才回 `4001`。

### 【新增客戶資料】

PATH

```
POST /api/_2000Customer/Create
```

REQ

```json
{
  "id": null,                          // 新增不帶
  "attribName": "TEST_A",              // 選填，32 字
  "name": "TEST_客戶A",                // 必填，64 字
  "taxInvoiceNumber": "12345678",      // 必填，8 字
  "owner": "TEST_王小明",              // 必填，32 字
  "contactPhone": "02-1234-5678",      // 選填，32 字
  "faxPhone": null,                    // 選填，32 字
  "staffChineseName": null,            // 選填，32 字，責任業務員
  "registeredAddress": "TEST_台北市",  // 必填，64 字
  "deliveryAddress": null,             // 選填，64 字
  "taxInvoiceAddress": null,           // 選填，64 字
  "bankName": null,                    // 選填，32 字
  "checkingAccount": null,             // 選填，32 字
  "remittanceAccount": null,           // 選填，32 字
  "payDays": "30",                     // 選填，8 字，付款票期
  "creditLine": 100000,                // 選填，0～1000000000
  "creditBalance": 50000,              // 選填，0～1000000000
  "lastDeliveryDate": "2026-10-01",    // 選填，yyyy-MM-dd
  "advance": 0,                        // 選填，0～1000000000，暫收款
  "invoiceForm": 1                     // 選填，0／1／2，預設 1
}
```

RSP

```json
{ "isSuccess": true, "errorCode": 0, "errorMessage": null, "data": "資料成功新增" }
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 4000 | 欄位值無效（`data` 為欄位錯誤清單） | `FieldValueIsInvalid` |
| 1008 | 無效的權杖 | `InvalidToken` |

- 備註：新增與修改共用同一流程，以 `id` 是否存在判斷。**現況落差**：`Create` 帶已存在的 `id` 會變成修改；統一編號重複不會被擋。

### 【修改客戶資料】

PATH

```
POST /api/_2000Customer/Edit
```

REQ：欄位同新增，`id` 必填。

```json
{
  "id": 1,                             // 必填
  "name": "TEST_客戶A（改）",          // 必填，其餘欄位規則同新增
  "taxInvoiceNumber": "12345678",
  "owner": "TEST_王小明",
  "registeredAddress": "TEST_台北市"
}
```

RSP

```json
{ "isSuccess": true, "errorCode": 0, "errorMessage": null, "data": "資料成功修改" }
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 4000 | 欄位值無效（`data` 為欄位錯誤清單） | `FieldValueIsInvalid` |
| 4001 | 找不到該資料（預期，現況未實作） | `NotFoundData` |
| 1008 | 無效的權杖 | `InvalidToken` |

- 備註：未帶的選填欄位會被覆寫為空，需送完整資料。**現況落差**：`id` 不存在時不回 `4001`，而是新增一筆並回「資料成功新增」。

### 【刪除客戶資料】

PATH

```
POST /api/_2000Customer/Delete?id=1
```

REQ

```
id=1    // 必填，客戶流水號（網址參數，不放 JSON 本文）
```

RSP

```json
{ "isSuccess": true, "errorCode": 0, "errorMessage": null, "data": "資料已刪除" }
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 4001 | 找不到該資料 | `NotFoundData` |
| 1008 | 無效的權杖 | `InvalidToken` |

- 備註：刪除後無法復原。已被其他單據引用的客戶，刪除行為尚未定義，待確認。

### 【取得客戶下拉選單】

PATH

```
GET /api/Shared/GetCustomerSelect
```

REQ：無

RSP

```json
{
  "isSuccess": true,
  "errorCode": 0,
  "errorMessage": null,
  "data": { "items": [ { "value": 1, "text": "TEST_客戶A" } ] }
}
```

| 錯誤碼 | 說明 | 常數 |
|---|---|---|
| 0 | 成功 | `None` |
| 1008 | 無效的權杖 | `InvalidToken` |

## 現況落差彙整

1. 列表 API 永遠回空清單。
2. 檢視 API 不依 `id` 篩選，查無資料不會回 `4001`。
3. 修改 API 對不存在的 `id` 會新增。
4. 新增 API 帶已存在的 `id` 會變成修改；統一編號重複不被擋。
5. 客戶被其他單據引用時的刪除行為未定義。
