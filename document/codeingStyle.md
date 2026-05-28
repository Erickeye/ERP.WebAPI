# ERP.WebAPI 後端 Coding Style

> 本文件依照目前專案架構整理，適用範圍包含 `Controller`、`Service / Interface`、`ViewModels`、`EntityModels`、共用 Helper / Extension 與 API 回傳格式。  
> 不包含前端 `ERP.Vue`。

## 1. 專案分層

目前後端以多專案方式分層：

| 專案 | 職責 |
| --- | --- |
| `ERP.WebAPI` | API 入口、Controller、Middleware、CustomAttributes、Program.cs 設定 |
| `ERP.Service` | 商業邏輯、資料存取流程、簽核/序號/共用服務 |
| `ERP.Library` | ViewModels、Enums、Extensions、Helpers、Attributes、共用回傳模型 |
| `ERP.EntityModels` | EF Core `ERPDbContext` 與資料表 Entity Models |
| `ERP.DB` | SQL Database Project 與資料表定義 |

新增功能時，應優先遵守以下依賴方向：

```text
ERP.WebAPI
  -> ERP.Service
      -> ERP.Library
      -> ERP.EntityModels
```

Controller 不直接操作 `ERPDbContext`，資料查詢與商業邏輯放在 Service。

## 2. 命名與資料夾規則

### 2.1 模組編號命名

目前業務模組多以功能編號命名，例如：

```text
_1000Company
_2000Customer
_4000Inventory
```

新增模組時，Controller、Service、ViewModel 建議保持同一組模組資料夾：

```text
src/ERP.WebAPI/Controllers/_4000Inventory/_4010PurchaseController.cs
src/ERP.Service/API/_4000Inventory/_4010PurchaseService.cs
src/ERP.Library/ViewModels/_4000Inventory/PurchaseVM.cs
```

### 2.2 Controller 命名

Controller 使用功能編號加業務名稱：

```csharp
public class _4010PurchaseController : ControllerBase
```

檔名需與類別名稱一致：

```text
_4010PurchaseController.cs
```

### 2.3 Service 與 Interface 命名

Service 介面與實作放在同一個檔案中，命名規則如下：

```csharp
public interface I_4010PurchaseService
{
}

public class _4010PurchaseService : I_4010PurchaseService
{
}
```

檔名：

```text
_4010PurchaseService.cs
```

`Program.cs` 會透過反射自動註冊 `ERP.Service` 底下符合 `I{ClassName}` 命名的 Service，因此新增 Service 時需確保：

- Interface 名稱為 `I` + Service 類別名稱。
- 類別不是 abstract。
- Namespace 在 `ERP.Service` 底下。

### 2.4 ViewModel 命名

ViewModel 依用途加後綴：

| 用途 | 命名 |
| --- | --- |
| 查詢條件 | `PurchaseSearchVM` |
| 列表/明細輸出 | `PurchaseVM`、`DayOffListVM` |
| 新增/修改輸入 | `PurchaseAddVM`、`DayOffInputVM` |
| 明細項目 | `PurchaseItemVM` |

同一功能的 VM 可放在同一個檔案中，例如 `PurchaseVM.cs`。

## 3. Controller 撰寫規範

Controller 應保持薄層，只負責：

- 定義 API route。
- 綁定 request model。
- 呼叫 Service。
- 回傳 `Ok(result)`。
- 加上 Swagger、驗證、Log、授權等 Attribute。

標準骨架：

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

Controller 層級：

```csharp
[Route("api/[controller]")]
```

Action 層級目前使用明確 Route：

```csharp
[HttpGet, Route("Index")]
[HttpGet, Route("Get")]
[HttpPost, Route("Add")]
[HttpPost, Route("Edit")]
[HttpDelete, Route("Delete")]
```

查詢條件使用 `[FromQuery]`：

```csharp
public async Task<IActionResult> Index([FromQuery] PurchaseSearchVM vm)
```

新增、修改、簽核等複雜資料使用 body model：

```csharp
public async Task<IActionResult> Add(PurchaseAddVM vm)
```

### 3.2 Attribute 使用規則

| Attribute | 使用時機 |
| --- | --- |
| `[ApiController]` | 每個 Controller 必加 |
| `[Route("api/[controller]")]` | 每個 Controller 必加 |
| `[SwaggerTag("...")]` | 描述 Controller 類別 |
| `[SwaggerOperation("...")]` | 描述每個 Action |
| `[ValidateModel]` | 有輸入 VM 且需要 DataAnnotations 驗證時使用 |
| `[Log(...)]` | 新增、修改、刪除、簽核等需要操作紀錄時使用 |
| `[AllowAnonymous]` | 明確允許匿名呼叫時使用 |

全站預設需要授權，只有登入、公開下拉選單或明確開放的 API 才加 `[AllowAnonymous]`。

## 4. Service 撰寫規範

Service 負責：

- 查詢與更新資料庫。
- 商業邏輯判斷。
- Entity 與 VM 轉換。
- 呼叫其他 Service。
- 回傳 `ResultModel<T>`。

### 4.1 Service 骨架

```csharp
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4010PurchaseService
    {
        Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm);
        Task<ResultModel<PurchaseVM>> Get(int id);
        Task<ResultModel<string>> Add(PurchaseAddVM vm);
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

所有 Service 方法應回傳 `ResultModel<T>`：

```csharp
Task<ResultModel<string>> Add(PurchaseAddVM vm);
Task<ResultModel<PurchaseVM>> Get(int id);
Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm);
```

成功：

```csharp
return ResultModel.Ok(data);
return ResultModel.Ok("Success");
```

失敗：

```csharp
return ResultModel.Error(ErrorCodeType.NotFoundData);
return ResultModel.Error(ErrorCodeType.InvalidApproval, "錯誤訊息");
```

呼叫其他 Service 後，應檢查結果：

```csharp
var result = await _approvalService.Approval(vm);
if (!result)
{
    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
}
```

### 4.3 Add / Edit 共用 Modify

新增與修改若欄位處理一致，使用私有 `Modify` 方法集中設定 Entity：

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

查詢型 API 使用 `AsNoTracking()`：

```csharp
var data = await _db.t_4010Purchase
    .AsNoTracking()
    .Where(x => x.Id == id)
    .Select(x => new PurchaseVM
    {
        Id = x.Id,
        No = x.No
    })
    .FirstOrDefaultAsync();
```

列表查詢應在資料庫端完成 `Where`、`Select`、排序與分頁，避免先 `ToListAsync()` 再於記憶體篩選大量資料。

修改型 API 需要更新子集合時，使用 `Include` 載入明細：

```csharp
var entity = await _db.t_4010Purchase
    .Include(x => x.t_4011PurchaseDetail)
    .Where(x => x.Id == vm.Id)
    .FirstOrDefaultAsync();
```

### 4.5 SaveChanges 規則

- 同一個商業流程盡量只在流程最後呼叫一次 `SaveChangesAsync()`。
- 若呼叫其他 Service 只改同一個 DbContext，可由外層流程統一儲存。
- 發生錯誤時直接回傳 `ResultModel.Error(...)`，不要繼續儲存。

## 5. 查詢、排序與分頁

### 5.1 SearchModel

需要分頁與排序的查詢 VM 繼承 `SearchModel`：

```csharp
public class PurchaseSearchVM : SearchModel
{
    [SwaggerParameter("進貨單號")]
    [SearchField("No", SearchCompare.Contains)]
    public string? No { get; set; }
}
```

`SearchModel` 已提供：

- `PageNumber`，預設 1。
- `PageSize`，預設 10。
- `SortColumn`。
- `SortDirection`，預設 `asc`。

### 5.2 SearchField

可使用 `[SearchField]` 讓 `SearchExpressionBuilder` 自動建立查詢條件：

```csharp
[SearchField("No", SearchCompare.Contains)]
public string? No { get; set; }

[SearchField("Supplier.Name", SearchCompare.Contains)]
public string? SupplierName { get; set; }

[SearchField("PurchaseDate", SearchCompare.GreaterThanOrEqual)]
public DateTime? PurchaseDateStrat { get; set; }

[SearchField("PurchaseDate", SearchCompare.LessThanOrEqual)]
public DateTime? PurchaseDateEnd { get; set; }
```

支援比較方式：

| Compare | 用途 |
| --- | --- |
| `Equal` | 等於 |
| `Contains` | 字串模糊查詢 |
| `GreaterThanOrEqual` | 大於等於 |
| `LessThanOrEqual` | 小於等於，日期會補到當日最後一刻 |

### 5.3 分頁與排序寫法

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

## 6. ViewModel 撰寫規範

ViewModel 放在：

```text
src/ERP.Library/ViewModels/{ModuleFolder}
```

### 6.1 驗證規則

輸入 VM 使用 DataAnnotations：

```csharp
public class PurchaseAddVM
{
    [SwaggerParameter("供應商識別碼")]
    [Required(ErrorMessage = "必填欄位")]
    public int SupplierId { get; set; }

    [SwaggerParameter("專案名稱")]
    [Required(ErrorMessage = "必填欄位")]
    [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
    public string? ProjectName { get; set; } = null!;
}
```

有使用 DataAnnotations 的 Action 要加 `[ValidateModel]`。  
驗證失敗時，由 `ValidateModelAttribute` 統一回傳 `ResultModel<Dictionary<string, List<string>>>`。

### 6.2 SwaggerParameter

API 文件欄位說明使用 `[SwaggerParameter]`：

```csharp
[SwaggerParameter("進貨單號")]
public string? No { get; set; }
```

若欄位只用於 MVC Display 或錯誤訊息，可使用 `[Display(Name = "...")]`。

### 6.3 Null 與預設值

集合屬性需初始化，避免 null reference：

```csharp
public List<PurchaseItemVM> Items { get; set; } = new List<PurchaseItemVM>();
```

字串依資料庫與業務需求決定是否允許 null。若寫入 Entity 時不可為 null，可在 Service 做預設值：

```csharp
entity.ProjectName = vm.ProjectName ?? "";
```

## 7. EntityModels 與資料庫

Entity 與 `ERPDbContext` 位於：

```text
src/EntityModels/ERP.EntityModels
```

原則：

- Entity class 對應資料表，不放商業邏輯。
- 商業邏輯放 Service。
- 查詢輸出不要直接回傳 Entity，應投影成 VM。
- 新增資料表後，需同步確認 `ERP.DB` SQL 定義、Entity Model 與 DbContext。

## 8. API 回傳與錯誤處理

API 統一回傳 HTTP 200 搭配 `ResultModel<T>`，登入授權失敗例外由 JWT / Middleware 處理。

成功格式：

```json
{
  "isSuccess": true,
  "errorCode": 0,
  "errorMessage": null,
  "data": {}
}
```

失敗格式：

```json
{
  "isSuccess": false,
  "errorCode": 1001,
  "errorMessage": "錯誤訊息",
  "data": null
}
```

錯誤代碼應使用 `ErrorCodeType`，避免在各 Service 自行定義魔術數字。

## 9. Swagger 分組

Swagger 分組由 `Program.cs` 的 `SwaggerGroupResolver` 依 Controller namespace 判斷：

```csharp
if (ns.Contains("Company"))
    return ApiGroupType.Company.ToString();

if (ns.Contains("Customer"))
    return ApiGroupType.Customer.ToString();

if (ns.Contains("Inventory"))
    return ApiGroupType.Inventory.ToString();
```

新增模組時需確認：

- Controller namespace 是否能被正確歸到 Swagger 分組。
- 若是新大模組，需新增 `ApiGroupType` 與 `SwaggerGroupResolver` 判斷。

## 10. Logging 與操作紀錄

需要記錄使用者操作的 Action 加上 `[Log]`：

```csharp
[Log(OperationActionType.Create, "建立進貨單")]
```

常見操作：

| 操作 | ActionType |
| --- | --- |
| 新增 | `OperationActionType.Create` |
| 檢視 | `OperationActionType.View` |
| 簽核 | `OperationActionType.Approval` |
| 撤銷簽核 | `OperationActionType.RevokeApproval` |

說明文字需與 `[SwaggerOperation]` 保持一致或語意接近，方便查 log。

## 11. 授權規則

全站預設套用 `[Authorize]`，設定位於 `Program.cs`：

```csharp
options.Filters.Add(new AuthorizeFilter(policy));
```

只有以下情境使用 `[AllowAnonymous]`：

- 登入。
- Refresh token 或公開認證流程。
- 不含敏感資料的公開下拉資料。
- 明確被定義為公開 API 的功能。

## 12. 共用工具使用原則

優先使用專案既有 Helper / Extension：

| 工具 | 用途 |
| --- | --- |
| `SearchExpressionBuilder` | 依 Search VM 產生查詢條件 |
| `SortHelper.GetColumns<T>()` | 產生可排序欄位 |
| `ApplySort(...)` | 套用排序 |
| `ToPagedResultAsync(...)` | 分頁 |
| `ExpressionAnd(...)` | 手動組合 Expression |
| `To235959()` | 日期補到當日最後時間 |
| `GetDisplayName<TEnum>()` | 取得 Enum 顯示名稱 |
| `ToEnumList()` | 產生 Enum 下拉選單資料 |

不要在不同 Service 重複撰寫已存在的共用邏輯。

## 13. C# 格式規範

格式以 `.editorconfig` 為準，重點如下：

- 編碼：UTF-8。
- 換行：CRLF。
- 縮排：4 spaces。
- `using` 放在 namespace 外。
- public method / property 使用 PascalCase。
- private field 使用 camelCase，依現有注入欄位慣例可使用底線前綴，例如 `_db`、`_service`。
- 常數使用 PascalCase。
- 大括號換行。
- 最大行寬建議 160。

範例：

```csharp
private readonly ERPDbContext _db;

public async Task<ResultModel<string>> Add(PurchaseAddVM vm)
{
    var entity = new t_4010Purchase();

    _db.t_4010Purchase.Add(entity);
    await _db.SaveChangesAsync();

    return ResultModel.Ok("Success");
}
```

## 14. 新增 API 檢查清單

新增一個後端 API 功能時，依序確認：

1. 在 `ERP.Library/ViewModels/{Module}` 新增或更新 VM。
2. 輸入 VM 加上 `[Required]`、`[StringLength]` 等驗證。
3. 查詢 VM 若需要分頁，繼承 `SearchModel`。
4. 查詢欄位加上 `[SearchField]` 與 `[SwaggerParameter]`。
5. 在 `ERP.Service/API/{Module}` 新增 Interface 與 Service。
6. Service 方法統一回傳 `ResultModel<T>`。
7. 查詢使用 `AsNoTracking()` 與 VM projection。
8. 新增/修改集中使用 `Modify` 處理可共用欄位。
9. 找不到資料時回傳 `ResultModel.Error(ErrorCodeType.NotFoundData)`。
10. 在 `ERP.WebAPI/Controllers/{Module}` 新增 Controller Action。
11. Action 加上 `[SwaggerOperation]`。
12. 有輸入驗證的 Action 加上 `[ValidateModel]`。
13. 需操作紀錄的 Action 加上 `[Log]`。
14. 確認 Service 命名符合自動 DI 註冊規則。
15. 確認 Swagger 分組正確。

## 15. 建議避免

- Controller 直接存取 `ERPDbContext`。
- API 直接回傳 Entity Model。
- Service 回傳裸型別，例如 `string`、`List<T>`，應包成 `ResultModel<T>`。
- 在 Controller 寫商業邏輯。
- 在多個 Service 複製相同查詢、排序、分頁邏輯。
- 查詢列表時先 `ToListAsync()` 再做篩選、排序、分頁。
- 使用魔術數字表示錯誤代碼或簽核狀態。
- 未加 `[ValidateModel]` 就依賴 DataAnnotations。
- 新增 Service 後命名不符合 `I{ClassName}`，導致 DI 無法自動註冊。

