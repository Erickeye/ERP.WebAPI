# ERP.WebAPI 後端 Coding Style

> 本文件依照 ERP.WebAPI 目前架構整理，適用範圍包含 `Controller`、`Service / Interface`、`ViewModels`、`EntityModels`、共用 Helper / Extension、API 回傳格式、Log、授權與外部服務。
> 前端請見 `document/vueCodingStyle.md`。
> 文件中標示「**現況落差**」的段落，代表程式碼目前尚未完全符合規範，新程式請依規範撰寫，舊程式不需為此另開重構。

## 1. 專案分層

| 專案 | 職責 |
| --- | --- |
| `ERP.WebAPI` | API 入口、Controller、Middleware、CustomAttributes、Filters、Program.cs 設定 |
| `ERP.Service` | 商業邏輯、資料存取流程、序號／簽核轉接／Redis／SFTP／外部 API 等服務 |
| `ERP.Approval` | 簽核引擎（獨立專案，見第 16 節） |
| `ERP.Library` | ViewModels、Enums、Extensions、Helpers、Attributes、共用回傳模型 |
| `ERP.EntityModels` | EF Core `ERPDbContext` 與資料表 Entity Models |
| `ERP.DB` | SQL Database Project 與資料表定義 |

依賴方向：

```text
ERP.WebAPI
  -> ERP.Service
      -> ERP.Approval
      -> ERP.Library
      -> ERP.EntityModels
```

Controller 不直接操作 `ERPDbContext`，資料查詢與商業邏輯放在 Service。

## 2. 命名與資料夾

### 2.1 模組編號命名

業務模組以功能編號命名：

```text
_1000Company
_2000Customer
_4000Inventory
```

新增功能時，Controller、Service、ViewModel 保持同一組模組資料夾：

```text
src/ERP.WebAPI/Controllers/_4000Inventory/_4010PurchaseController.cs
src/ERP.Service/API/_4000Inventory/_4010PurchaseService.cs
src/ERP.Library/ViewModels/_4000Inventory/PurchaseVM.cs
```

> 注意：ViewModel 檔名／類別名**不加編號前綴**（`PurchaseVM`），只有 Controller 與 Service 加。

### 2.2 Controller 命名

```csharp
public class _4010PurchaseController : ControllerBase
```

檔名與類別名稱一致（`_4010PurchaseController.cs`）。Route 為 `api/[controller]`，所以網址會是 `api/_4010Purchase/Index`，前端串接時請照此路徑。

### 2.3 Service 與 Interface 命名

Interface 與實作放在同一個檔案：

```csharp
public interface I_4010PurchaseService { }
public class _4010PurchaseService : I_4010PurchaseService { }
```

`Program.cs` 以反射自動註冊 Service（`AddScoped`），條件如下，新增 Service 必須同時滿足：

- 類別 namespace 以 `ERP.Service` 開頭。
- 類別實作名為 `I{ClassName}` 的 Interface。
- 類別不是 `abstract`、不是 interface。
- **每個類別建議只實作一個 Interface**：註冊時是取 `GetInterfaces().FirstOrDefault()`，實作多個 Interface 時註冊到哪一個不可預期。
- 需要特殊生命週期（Singleton、Options、HttpClient）的服務，不要走自動註冊，改在 `Program.cs` 或專屬的 `AddXxxModule()` 擴充方法明確註冊（例如 `AddApprovalModule()`）。

### 2.4 ViewModel 命名

| 用途 | 命名 |
| --- | --- |
| 查詢條件 | `PurchaseSearchVM` |
| 列表／明細輸出 | `PurchaseVM`、`DayOffListVM` |
| 新增／修改輸入 | `PurchaseAddVM`、`DayOffInputVM` |
| 明細項目 | `PurchaseItemVM` |

同一功能的 VM 可放在同一個檔案（如 `PurchaseVM.cs`）。

### 2.5 Entity 命名

資料表 Entity 為 DB-first scaffold 產物，沿用資料表名稱（如 `t_4010Purchase`、`t_1710ActionInfo`）。**不手改 scaffold 產出的 Entity**；改欄位時改 `ERP.DB` 後重新 scaffold（指令見 `README.md`）。

## 3. Controller 規範

Controller 保持薄層，只負責：定義 route、綁定 request model、呼叫 Service、回傳 `Ok(result)`、加上 Swagger／驗證／Log／授權 Attribute。

```csharp
using ERP.Library.Enums;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("進貨單")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4010PurchaseController : ControllerBase
    {
        private readonly I_4010PurchaseService _service;

        public _4010PurchaseController(I_4010PurchaseService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢進貨單列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] PurchaseSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("建立進貨單")]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立進貨單")]
        public async Task<IActionResult> Add(PurchaseAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }
    }
}
```

### 3.1 Route 規則

Controller 層級固定 `[Route("api/[controller]")]`；Action 層級使用明確 Route：

```csharp
[HttpGet, Route("Index")]    // 列表（SearchModel 分頁）
[HttpGet, Route("Get")]      // 單筆
[HttpPost, Route("Add")]
[HttpPost, Route("Edit")]
[HttpDelete, Route("Delete")]
```

列表 Action 統一命名 `Index`（本專案慣例；TeacherMgmt 用 `Search`，兩專案不需強求一致）。查詢條件用 `[FromQuery]`；新增、修改、簽核等複雜資料用 body model。

### 3.2 Attribute 使用規則

| Attribute | 使用時機 |
| --- | --- |
| `[ApiController]` | 每個 Controller 必加 |
| `[Route("api/[controller]")]` | 每個 Controller 必加 |
| `[SwaggerTag("...")]` | 描述 Controller 類別 |
| `[SwaggerOperation("...")]` | 描述每個 Action |
| `[ValidateModel]` | 有輸入 VM 且依賴 DataAnnotations 驗證時 **必加**（見下方說明） |
| `[Log(...)]` | 新增、修改、刪除、匯出、簽核等需要操作紀錄時 |
| `[PermissionAuthorize(...)]` | 需要角色權限碼的 Action（見第 11 節） |
| `[RequireCaptcha]` | 需要驗證碼的 Action（見第 12 節） |
| `[AllowAnonymous]` | 明確允許匿名呼叫時 |

> `Program.cs` 設定了 `SuppressModelStateInvalidFilter = true`，框架**不會**自動回 400。沒有 `[ValidateModel]` 的 Action，DataAnnotations 驗證失敗時會直接放行進 Service。

## 4. Service 規範

Service 負責：查詢與更新資料庫、商業邏輯判斷、Entity 與 VM 轉換、呼叫其他 Service、回傳 `ResultModel<T>`。

### 4.1 Service 骨架

Interface 與每個公開方法都加上 `<summary>` 中文註解（功能名稱），商業邏輯較多處加行內中文註解說明「為什麼」。

```csharp
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    /// <summary>
    /// 進貨單-服務
    /// </summary>
    public interface I_4010PurchaseService
    {
        /// <summary>
        /// 查詢列表
        /// </summary>
        Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm);

        /// <summary>
        /// 取得單筆
        /// </summary>
        Task<ResultModel<PurchaseVM>> Get(int id);

        /// <summary>
        /// 新增
        /// </summary>
        Task<ResultModel<string>> Add(PurchaseAddVM vm);

        /// <summary>
        /// 修改
        /// </summary>
        Task<ResultModel<string>> Edit(PurchaseAddVM vm);
    }

    public class _4010PurchaseService : I_4010PurchaseService
    {
        private readonly ERPDbContext _db;

        public _4010PurchaseService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PurchaseVM>> Get(int id)
        {
            var data = await _db.t_4010Purchase
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new PurchaseVM
                {
                    Id = x.Id,
                    No = x.No
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            return ResultModel.Ok(data);
        }
    }
}
```

### 4.2 回傳規則

所有 Service 公開方法回傳 `ResultModel<T>`，不回傳裸型別：

```csharp
return ResultModel.Ok(data);                    // 帶資料
return ResultModel.Ok();                        // 無資料，Data = "Success"
return ResultModel.Ok(listItems);               // List<T> 會包成 ListResult<T>（Items）
return ResultModel.Error(ErrorCodeType.NotFoundData);
return ResultModel.Error(ErrorCodeType.InvalidRequest, "自訂錯誤訊息");
```

- `ResultModel.Error(code)` 不傳訊息時，訊息取 `ErrorCodeType` 的 `[Display(Name=...)]`。
- `ResultModel<T>` 可隱式轉 `bool`，呼叫其他 Service 後這樣檢查並轉傳錯誤：

```csharp
var result = await _approvalService.Approval(vm);
if (!result)
{
    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
}
```

### 4.3 Add / Edit 共用 Modify

新增與修改欄位處理一致時，用私有 `Modify` 集中設定 Entity：

```csharp
public async Task<ResultModel<string>> Add(PurchaseAddVM vm)
{
    var entity = new t_4010Purchase
    {
        CreateTime = DateTime.Now
    };

    var result = Modify(vm, entity);
    if (!result)
    {
        return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
    }

    _db.t_4010Purchase.Add(entity);
    await _db.SaveChangesAsync();

    return ResultModel.Ok(entity.No!);
}

private ResultModel<string> Modify(PurchaseAddVM vm, t_4010Purchase entity)
{
    entity.ProjectName = vm.ProjectName ?? "";
    entity.Note = vm.Note;

    return ResultModel.Ok();
}
```

### 4.4 EF Core 查詢規則

- 查詢型 API 使用 `AsNoTracking()` 並直接投影成 VM（`Select`）。
- 列表在資料庫端完成 `Where`、`Select`、排序與分頁，不要先 `ToListAsync()` 再於記憶體篩選。
- 修改型 API 需更新子集合時，用 `Include` 載入明細。
- 複雜報表或既有預存程序，使用 `Database.StoredProcedureQueryAsync<T>(...)`（`ERP.Library.Extensions.StoredProcedureExtension`），不要手寫字串拼接 SQL（避免 SQL Injection）。該擴充預設 command timeout 為 3600 秒，長查詢請明確傳入 `commandTimeout`。

### 4.5 SaveChanges 與交易

- 同一個商業流程盡量只在最後呼叫一次 `SaveChangesAsync()`。
- 呼叫其他 Service 若共用同一個 `ERPDbContext`（Scoped），由外層流程統一儲存。
- 需要「多次 SaveChanges 仍須全成功或全失敗」時（例如先取序號再寫單據），使用明確的 `BeginTransactionAsync()`，不要依賴巧合順序。
- 發生錯誤時直接 `return ResultModel.Error(...)`，不要繼續儲存。

## 5. 查詢、排序與分頁

### 5.1 SearchModel

需要分頁與排序的查詢 VM 繼承 `SearchModel`，其提供：`PageNumber`（預設 1，小於 1 自動校正為 1）、`PageSize`（預設 10）、`SortColumn`、`SortDirection`（預設 `asc`）。

```csharp
public class PurchaseSearchVM : SearchModel
{
    [Display(Name = "進貨單號")]
    [SearchField("No", SearchCompare.Contains)]
    public string? No { get; set; }

    [Display(Name = "供應商名稱")]
    [SearchField("Supplier.Name", SearchCompare.Contains)]
    public string? SupplierName { get; set; }

    [Display(Name = "進貨日期起")]
    [SearchField("PurchaseDate", SearchCompare.GreaterThanOrEqual)]
    public DateTime? PurchaseDateStart { get; set; }

    [Display(Name = "進貨日期迄")]
    [SearchField("PurchaseDate", SearchCompare.LessThanOrEqual)]
    public DateTime? PurchaseDateEnd { get; set; }
}
```

| Compare | 用途 |
| --- | --- |
| `Equal` | 等於 |
| `Contains` | 字串模糊查詢 |
| `GreaterThanOrEqual` | 大於等於 |
| `LessThanOrEqual` | 小於等於，日期會補到當日最後一刻 |

### 5.2 分頁與排序寫法

```csharp
var filter = SearchExpressionBuilder.Build<t_4010Purchase>(vm);

var query = _db.t_4010Purchase
    .AsNoTracking()
    .Where(filter)
    .Select(x => new PurchaseVM
    {
        Id = x.Id,
        No = x.No
    });

if (!string.IsNullOrWhiteSpace(vm.SortColumn))
{
    query = query.ApplySort(vm, SortHelper.GetColumns<PurchaseVM>());
}

var pageResult = await query.ToPagedResultAsync(vm);
return ResultModel.Ok(pageResult);
```

`SortHelper.GetColumns<T>()` 提供可排序欄位白名單，避免前端傳入任意欄位名稱。若需要排序欄位別名或預設排序，放在 `ERP.Library/QuerySort`（如 `InventorySort`）。

## 6. ViewModel 規範

放在 `src/ERP.Library/ViewModels/{ModuleFolder}`。

```csharp
public class PurchaseAddVM
{
    [Display(Name = "供應商識別碼")]
    [Required(ErrorMessage = "必填欄位")]
    public int SupplierId { get; set; }

    [Display(Name = "專案名稱")]
    [Required(ErrorMessage = "必填欄位")]
    [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
    public string? ProjectName { get; set; }

    public List<PurchaseItemVM> Items { get; set; } = new();
}
```

- 輸入 VM 使用 DataAnnotations，Action 需加 `[ValidateModel]`；驗證失敗由 `ValidateModelAttribute` 統一回傳 `ResultModel<Dictionary<string, List<string>>>`。
- 欄位說明一律用 `[Display(Name = "...")]`（System.ComponentModel.DataAnnotations）；`DisplayNameSchemaFilter` 會把它帶進 Swagger，匯出（ClosedXML）也會用它當欄位標題。
- 集合屬性要初始化，避免 null reference。
- 字串是否允許 null 依資料庫與業務決定；Entity 不可為 null 時，在 Service 給預設值（`vm.X ?? ""`）。
- 不要把 Entity 直接當 VM 回傳。

## 7. EntityModels 與資料庫

- Entity 對應資料表，不放商業邏輯；商業邏輯放 Service。
- 新增或異動資料表後，同步確認 `ERP.DB` SQL 定義、Entity Model 與 `ERPDbContext`（使用 DB-first scaffold，不手改產出檔）。
- 連線字串、Redis、JWT、SFTP 等敏感設定放 `appsettings.Development.json`（已 gitignore 為本機用），repo 內只放 `appsettings.Development.example.json` 佔位範例，**不提交真實密碼**。

## 8. API 回傳與錯誤處理

API 統一回傳 HTTP 200 搭配 `ResultModel<T>`（JWT 驗證失敗等框架層級例外除外）。

```json
{ "isSuccess": true,  "errorCode": 0,    "errorMessage": null,   "data": {} }
{ "isSuccess": false, "errorCode": 1003, "errorMessage": "權限不足", "data": null }
```

- 錯誤代碼一律使用 `ErrorCodeType`，不要在 Service 自訂魔術數字。新增代碼時**確認數值不重複**（**現況落差**：`CaptchaRequired` 與 `CaptchaExpired` 目前同為 1011，前端無法區分，新增時請避免這種情況）。
- 業務可預期的失敗（找不到資料、狀態不允許、重複）回傳 `ResultModel.Error(...)`，**不要用 throw 當流程控制**。
- 未預期例外由 `ExceptionMiddleware` 統一轉成 `ErrorCodeType.Exception`／`DbRelationError`。**現況落差**：該 Middleware 的 `_logger.LogError` 目前被註解，例外被吞掉而沒有任何紀錄；新增或修改這段時應恢復記錄例外，否則線上問題無從追查。

## 9. Swagger 分組

分組由 `Program.cs` 的 `SwaggerGroupResolver` 依 Controller namespace 判斷（`Company`／`Customer`／`Inventory`，其餘歸 `Home`）。新增大模組時：

1. `ApiGroupType` 新增列舉，並加上 `[Display(Name = "...")]`（Swagger 下拉顯示名稱）。
2. `SwaggerGroupResolver.Resolve` 加入對應 namespace 判斷。
3. 確認 Swagger UI 的群組下拉可看到新模組。

## 10. Logging 與操作紀錄

需要記錄使用者操作的 Action 加上 `[Log]`，說明文字與 `[SwaggerOperation]` 保持一致或語意接近：

```csharp
[Log(OperationActionType.Create, "建立進貨單")]
```

| 操作 | ActionType |
| --- | --- |
| 檢視／查詢 | `View` |
| 新增 | `Create` |
| 編輯 | `Edit` |
| 刪除 | `Delete` |
| 匯出 | `Export` |
| 匯入 | `Import` |
| 簽核／撤銷簽核 | `Approval`／`RevokeApproval` |
| 重設密碼 | `ResetSw` |

運作方式：`ActionLoggingMiddleware` 讀取 Endpoint 上的 `LogAttribute`，寫入 `t_1710ActionInfo`（帳號取自 JWT claim `account`、IP、路徑、`ActionType`、`Memo = 說明[HTTP Method]`）。注意事項：

- `KeyId` 目前只讀 query string 的 `id`；body 傳 id 的 Action 不會記到 KeyId。
- Log 在 Action 執行**之前**寫入，記錄的是「有人呼叫」而非「操作成功」。
- Log 寫入失敗只記錄錯誤、不影響主流程。
- 若 `ActionType` 沒有適合的列舉，先補列舉再使用，不要借用語意不符的類型。

應用程式記錄（Serilog）：使用 `ILogger<T>` 注入，訊息用結構化範本 `_logger.LogError(ex, "訊息 {Id}", id)`，不要字串插值；**不要記錄密碼、Token、完整身分證字號等敏感資料**。

## 11. 認證與授權

- 全站預設套用 `AuthorizeFilter`（`RequireAuthenticatedUser`），只有登入、Refresh token、公開下拉資料或明確公開的 API 才加 `[AllowAnonymous]`。
- 角色權限使用 `[PermissionAuthorize(permissionCodes...)]`：從 JWT claim `permissions`（JSON）判斷是否具備指定權限碼，不足時回傳 `ErrorCodeType.PermissionDenied`。新權限碼先在角色權限相關 Enum／資料中定義，再套用到 Action。
- 取得目前登入者**一律透過 `ICurrentUserService`**（`ERP.Service.API.AMS`，提供 UserId、UserName、RoleId、Role），不要在各 Service 自行讀取 `HttpContext.User.Claims`。
- 資料隔離（只看自己的資料）要在 Service 以 `ICurrentUserService.UserId` 強制改寫查詢條件，**不可只信任前端傳來的 id**。
- 密碼處理使用 `PasswordHelper`，不要自行實作雜湊；密碼規則錯誤使用 `PasswordXxx` 系列 `ErrorCodeType`。

## 12. 驗證碼（Redis Captcha）

需要驗證碼的 API 在 Action 上加 `[RequireCaptcha]`（`TypeFilterAttribute` → `RequireCaptchaFilter`，Swagger 透過 `RequireCaptchaOperationFilter` 顯示必填驗證碼欄位）。

- 驗證碼的產生與驗證在 `CaptchaService`，狀態存放 `RedisService`（`IConnectionMultiplexer` 為 Singleton，設定在 `RedisSettings`）。
- 不要在 Controller／Service 自行寫驗證碼比對邏輯。
- 驗證碼錯誤統一使用 `ErrorCodeType.CaptchaRequired`／`CaptchaExpired`／`InvalidCaptcha`。

## 13. 檔案、匯出與外部服務

### 13.1 匯出工具選擇

| 需求 | 使用工具 | 位置 |
| --- | --- | --- |
| Excel 清單匯出（依 VM 的 `[Display]` 當標題） | ClosedXML | `ERP.Library/Extensions/ClosedXmlToolExtension` |
| 套版的 Excel／Word（`Templates/*.xlsx`） | Aspose.Cells／Aspose.Words | `ERP.Library/Extensions/AsposeToolExtension` |
| 版面精緻的 PDF 報表 | QuestPDF | `ERP.Service/Extension/QuestPdfToolExtension` |

- 匯出 Action 加 `[Log(OperationActionType.Export, "...")]`。
- 樣板檔放 `ERP.WebAPI/Templates` 並設定複製到輸出目錄。
- Aspose 為商業授權元件，新增使用處前先確認授權與浮水印行為。

### 13.2 SFTP

檔案上傳、下載與清單由 `SftpService`（SSH.NET）處理，連線設定在 `SftpConfig`。路徑與檔名需驗證，避免 `..` 路徑穿越；不要把真實主機帳密寫進 repo。

### 13.3 外部 HTTP API

呼叫外部 API 使用注入的 `HttpClient`（`AddHttpClient()`）搭配 `HttpClientExtension`（`GetJsonResultAsync`／`PostJsonResultAsync`），結果已包成 `ResultModel<T>`。外部 API 的位址與選項放 Options（參考 `JsonPlaceholderApiOptions`），不要寫死在程式碼內。

## 14. 共用工具

優先使用既有 Helper／Extension，不要重複實作：

| 工具 | 用途 |
| --- | --- |
| `SearchExpressionBuilder` | 依 Search VM 與 `[SearchField]` 產生查詢條件 |
| `SortHelper.GetColumns<T>()` | 產生可排序欄位白名單 |
| `ApplySort(...)` | 依 `SearchModel` 套用排序 |
| `ToPagedResultAsync(...)` | 產生分頁結果 |
| `ExpressionAnd(...)` | 手動組合 Expression |
| `To235959()` | 日期補到當日最後時間 |
| `GetDisplayName()`／`GetDisplayName<T>()` | 取得 Enum 顯示名稱 |
| `ToEnumList()` | 產生 Enum 下拉選單資料 |
| `StoredProcedureQueryAsync<T>(...)` | 執行預存程序 |
| `SerialService` | 單據序號產生 |
| `SharedService` | 下拉選單等跨模組共用查詢 |

## 15. C# 格式規範

格式以根目錄 `.editorconfig` 為準，重點：

- 編碼 UTF-8；縮排 4 spaces；大括號換行（Allman）。
- `using` 放在 namespace 外；`System` 開頭排前面。
- public 成員 PascalCase；private 欄位 camelCase 並加底線前綴（`_db`、`_service`）；常數 PascalCase。
- 專案啟用 `Nullable` 與 `ImplicitUsings`，不要用 `!` 硬壓掉 null 警告，除非能確認非 null。
- 最大行寬建議 160。
- 本專案使用 block-scoped namespace（`namespace X { ... }`），新檔案沿用，不要混用 file-scoped。

## 16. 簽核模組（ERP.Approval）

簽核引擎已抽成獨立專案，業務模組**不要複製簽核邏輯**，只實作轉接：

1. 新增簽核目標的處理器，實作 `IApprovalTargetHandler`（`TableType`、`ExistsAsync`、`OnApprovedAsync`、`OnVoidedAsync`），放在 `ERP.Service/API/Approval`（參考 `PurchaseApprovalTargetHandler`、`DayoffApprovalTargetHandler`）。
2. 在 `TableType` 列舉新增對應單據類型，並於 `AddApprovalServiceAdapters()` 註冊處理器。
3. 簽核流程設定走 `IApprovalSettingsService`，流程推進走 `ApprovalWorkflowService`；目前使用者資訊由 `IApprovalUserContext` 提供。
4. 簽核動作的 Action 加 `[Log(OperationActionType.Approval, ...)]` 或 `RevokeApproval`。
5. `OnApprovedAsync`／`OnVoidedAsync` 在簽核結果確定後被呼叫，內部要自行處理失敗並回傳 `ResultModel`，不可吞掉錯誤。

## 17. 測試（規劃中）

目前專案尚未建立測試專案。建立後依下列方向（詳細見未來的 `src/ERP.Tests/README.md`）：

- xUnit + `Microsoft.EntityFrameworkCore.InMemory`，每個測試用隨機名稱建立獨立資料庫。
- 以 `FakeCurrentUserService` 取代 `ICurrentUserService`。
- 修 bug 時順手補一支回歸測試，不追求一次衝覆蓋率。
- InMemory 不驗證 SQL Server 專屬語法（預存程序、raw SQL、collation），此類邏輯另用整合測試。

## 18. 新增 API 檢查清單

1. 在 `ERP.Library/ViewModels/{Module}` 新增或更新 VM。
2. 輸入 VM 加上 `[Display]`、`[Required]`、`[StringLength]` 等驗證。
3. 查詢 VM 若需要分頁，繼承 `SearchModel`，查詢欄位加上 `[SearchField]`。
4. 在 `ERP.Service/API/{Module}` 新增 Interface 與 Service，Interface 與方法加 `<summary>`。
5. Service 類別明確實作 `I{ClassName}`，且只實作該一個 Interface。
6. Service 方法統一回傳 `ResultModel<T>`。
7. 查詢使用 `AsNoTracking()` 與 VM projection，排序白名單使用 `SortHelper`。
8. 新增／修改集中使用 `Modify` 處理共用欄位。
9. 找不到資料回傳 `ResultModel.Error(ErrorCodeType.NotFoundData)`。
10. 涉及「只能看自己」的資料，使用 `ICurrentUserService` 強制限縮。
11. 在 `ERP.WebAPI/Controllers/{Module}` 新增 Controller Action。
12. Action 加上 `[SwaggerOperation]`；有輸入驗證加 `[ValidateModel]`；需操作紀錄加 `[Log]`；需權限加 `[PermissionAuthorize]`。
13. 確認 Swagger 分組正確（第 9 節）。
14. 適當加上中文註解，尤其是商業邏輯較多的地方。
15. 若有新增資料表：`ERP.DB` → scaffold → 確認 Entity 與 DbContext。

## 19. 建議避免

- Controller 直接存取 `ERPDbContext`，或在 Controller 寫商業邏輯。
- API 直接回傳 Entity Model。
- Service 回傳裸型別（`string`、`List<T>`），應包成 `ResultModel<T>`。
- 在多個 Service 複製相同查詢、排序、分頁邏輯。
- 查詢列表時先 `ToListAsync()` 再做篩選、排序、分頁。
- 使用魔術數字表示錯誤代碼或簽核狀態。
- 未加 `[ValidateModel]` 就依賴 DataAnnotations。
- 新增 Service 後命名不符 `I{ClassName}`，或一個類別實作多個 Interface，導致 DI 註冊錯誤。
- 在各處自行讀取 JWT claim，而不用 `ICurrentUserService`。
- 把密碼、Token、連線字串提交進 repo，或寫進 Log。
- 用字串拼接組 SQL。
- 吞掉例外而不記錄。
