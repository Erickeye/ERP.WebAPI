---
name: backend-coding-style
description: Apply the ERP.WebAPI backend coding style when creating, modifying, reviewing, or explaining .NET Web API code in the layered ERP projects (ERP.WebAPI, ERP.Service, ERP.Library, ERP.EntityModels, ERP.DB, ERP.Approval); Controller, Service, ViewModel, ResultModel, Swagger, logging, validation, authorization, captcha, approval, export, EF Core query, helper, extension, sorting, filtering, or pagination work.
---

# Backend Coding Style

讓 ERP.WebAPI 的後端工作維持一致的分層與慣例。

## 必讀來源

修改、審查或產生後端程式碼前，先讀 `references/codingStyle.md`（本 skill 資料夾內）。該檔是本專案後端風格的唯一來源；本 skill 只固定執行順序與完成前檢查，不複製整份規範。

## 工作流程

1. 判斷影響的層級：WebAPI、Service、Library、EntityModels、DB、Approval。
2. 遵守依賴方向：WebAPI -> Service -> (Approval / Library / EntityModels)。Controller 不直接使用 `ERPDbContext`，也不寫商業邏輯。
3. Controller 保持薄層：Route 綁定、Attribute、呼叫 Service、`Ok(result)`。模組編號命名（如 `_4010PurchaseController`）、Route 為 `api/[controller]`，列表 Action 叫 `Index`。
4. Service 負責查詢／更新、商業邏輯、Entity 與 VM 轉換，所有公開方法回傳 `ResultModel<T>`，失敗用 `ErrorCodeType`；Interface 與方法加中文 `<summary>`。
5. 先用既有 Helper／Extension：`SearchModel`、`[SearchField]`、`SearchExpressionBuilder`、`SortHelper`、`ApplySort`、`ToPagedResultAsync`、`StoredProcedureQueryAsync`、`SerialService`、`SharedService`，不要重寫同類邏輯。
6. Action 屬性要齊：`[SwaggerOperation]`；有輸入驗證加 `[ValidateModel]`（專案關閉了自動 400，沒加就不會驗證）；需操作紀錄加 `[Log]`；需權限加 `[PermissionAuthorize]`；需驗證碼加 `[RequireCaptcha]`。
7. 取得登入者一律用 `ICurrentUserService`；「只看自己」的資料在 Service 強制以 UserId 限縮，不信任前端傳入的 id。
8. 簽核相關：只實作 `IApprovalTargetHandler` 轉接並註冊於 `AddApprovalServiceAdapters()`，不複製簽核流程邏輯。
9. 新增資料表：`ERP.DB` -> DB-first scaffold，不手改 Entity／DbContext。
10. 新增 Service 前確認自動 DI 規則：Interface 為 `I{ClassName}`、namespace 在 `ERP.Service` 底下、非 abstract、只實作一個 Interface。
11. 新增大模組時同步 `ApiGroupType` 與 `SwaggerGroupResolver`。

## 完成前檢查

- 以 `rg` 確認 Controller 沒有注入 `ERPDbContext`、Service 沒有回傳裸型別。
- 確認新 Action 的 Attribute 與 `references/codingStyle.md` 第 18 節檢查清單一致。
- 確認沒有字串拼接 SQL、沒有把密碼／Token／連線字串寫進程式碼或 Log。
- 確認錯誤沒有被吞掉（catch 後要記錄或回傳 `ResultModel.Error`）。

## 驗證

除非使用者明確要求，修改後端程式碼後不編譯、不執行測試；只做靜態檢查（`rg`、讀檔、diff review）並在交付時說明「僅完成靜態檢查」。

## 例外處理

`references/codingStyle.md` 標示「現況落差」的既有程式，只在被修改到該區域時順手收斂。不得因此擴大修改範圍；若需改動 Program.cs 全域設定、Middleware 或 `ErrorCodeType` 既有數值，先向使用者確認。
