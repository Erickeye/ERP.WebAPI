# 程式碼導覽

新 session 先讀這份，再決定要讀哪些檔案；不要從 `src/` 全面掃描。術語見 `GLOSSARY-MAP.md`。

## 專案職責（`src/`）

| 專案 | 放什麼 |
|---|---|
| `ERP.WebAPI` | Controller（依 `_1000Company` / `_2000Customer` / `_4000Inventory` / `Approval` 等資料夾分群）、`Program.cs`、Filter、Middleware、`[PermissionAuthorize]` |
| `ERP.Service` | 業務邏輯 `API/<分群>/_NNNNXxxService.cs`（介面與實作同檔）、`API/Approval/` 的簽核轉接器 |
| `ERP.Approval` | 通用簽核引擎：`ApprovalWorkflowService`、`IApprovalTargetHandler`；引擎不維護單據狀態 |
| `ERP.Library` | Enum（`Enums/`）、ViewModel（`ViewModels/`）、Helper、Extension |
| `EntityModels` | EF Core：`ERP.EntityModels/Models/`（資料表類別）、`Context/ERPDbContext.cs` |
| `ERP.DB` | SQL Server 資料庫專案（`dbo/Tables/*.sql`）；新資料表另附部署腳本於 `document/sql/` |
| `ERP.Tests` | xUnit；每個 Service 一個測試檔，輔助在 `Support/` |
| `ERP.Vue` | 前端：`views/`、`api/`、`utils/`、`router/index.js`、選單在 `components/DashboardLayout.vue` |

命名慣例：服務與控制器以資料表編號開頭（例如 `_1060BusinessTrip`），資料表 `t_1060BusinessTrip`。細節規範見 `document/codingStyle.md`、`document/vueCodingStyle.md`。

## 自動接線，不用手動註冊

- `Program.cs` 會掃描 `ERP.Service` 命名空間下「類別 `Xxx` 實作介面 `IXxx`」的型別並註冊為 Scoped。新服務只要遵守 `IXxx`／`Xxx` 命名即可，不必改 `Program.cs`。
- 簽核目標轉接器**不在**自動接線範圍，要手動加（見下）。

## 新增一個簽核單據模組的檢查清單

以出差單為範例（commit `9f0ba48`、`2dbfe07` 是完整參考）：

1. `ERP.Library/Enums/Other/TableType.cs`：新增單據類型。
2. `ERP.Library/Enums/Login/PermissionType.cs`：新增功能權限（出差單 = 1060、出差單管理 = 1061）。
3. `ERP.Library/Enums/<分群>/XxxStatus.cs`、`ViewModels/<分群>/XxxVM.cs`：狀態與 VM。
4. 資料表：`EntityModels/.../Models/t_NNNNXxx.cs`、`ERPDbContext.cs` 加 `DbSet` 與設定、`ERP.DB/dbo/Tables/t_NNNNXxx.sql`、`ERP.DB.sqlproj` 加入、部署腳本放 `document/sql/`。
5. `ERP.Service/API/<分群>/_NNNNXxxService.cs`：單據狀態在此維護，簽核動作（送簽、核准、拒絕、撤銷、作廢）與狀態在同一交易（見 `docs/inventory/adr/0001`、`docs/attendance/adr/0001`）。單號用 `ISerialService`。
6. `ERP.Service/API/Approval/XxxApprovalTargetHandler.cs`，並在 `ApprovalServiceCollectionExtensions.cs` 手動註冊 `IApprovalTargetHandler`。
7. `ERP.WebAPI/Controllers/<分群>/_NNNNXxxController.cs`：專屬簽核端點。
8. `ERP.WebAPI/Controllers/Approval/ApprovalController.cs`：把新類型加進 `ModuleHandledTypes`，擋掉通用簽核端點。
9. 前端：`api/xxxApi.js`、`views/`、`router/index.js`、`DashboardLayout.vue` 選單。
10. 測試與文件：`ERP.Tests/XxxServiceTests.cs`；詞彙表與 ADR 放 `docs/<context>/`。

## 測試輔助（`ERP.Tests/Support/`）

- `TestDb`：SQLite 記憶體庫，同一連線可建多個 DbContext；`TestERPDbContext` 補上 SQLite 不支援的 rowversion 與預設值行為。
- `Fakes.cs`：`FakeCurrentUser`、`FakeSerialService`、`FakeApprovalWorkflowService`（測 Service 時隔離引擎）。
- `ApprovalEngineHarness`：接上真實簽核引擎（測引擎行為或需要真實簽核紀錄時用）。
- `SeedData`：共用種子資料。
- 執行方式見 `docs/agents/build-and-test.md`。測試用 SQLite，不等於 SQL Server。

## 容易踩的地方

- 通用 `ApprovalController` 對領料單、出差單的五個簽核動作會直接擋下；走專屬端點。
- 簽核引擎只提供 `OnApproved`／`OnVoided` 鉤子，不記單據狀態，也沒有通知。
- 前端不要直接讀寫 `localStorage`，用 `utils/auth.js`；HTTP 一律走 `api/http.js`。
