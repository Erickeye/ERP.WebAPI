using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 領料單服務的行為測試：建立／編輯／刪除、簽核狀態流轉、最終核准才扣庫存、
/// 庫存不足、整筆回滾、並行衝突、作廢回補。
/// </summary>
public class MaterialIssueServiceTests : IDisposable
{
    private readonly TestDb _testDb = new();
    private readonly FakeApprovalWorkflowService _approval = new();
    // 同一個測試內所有服務共用同一個流水號產生器，避免兩張單據拿到相同單號
    private readonly FakeSerialService _serial = new();
    private readonly SeedData.Basics _basics;
    private readonly t_4000Inventory _inventoryA;
    private readonly t_4000Inventory _inventoryB;

    public MaterialIssueServiceTests()
    {
        using var db = _testDb.CreateContext();
        _basics = SeedData.CreateBasics(db);
        _inventoryA = SeedData.CreateInventory(db, _basics, "A001", 10m);
        _inventoryB = SeedData.CreateInventory(db, _basics, "B001", 5m);
    }

    public void Dispose() => _testDb.Dispose();

    private (I_4030MaterialIssueService Service, TestERPDbContext Db) CreateService()
    {
        var db = _testDb.CreateContext();
        var service = new _4030MaterialIssueService(
            db,
            new FakeCurrentUser { UserId = 7 },
            _serial,
            _approval,
            new StockMovementRecorder(db));
        return (service, db);
    }

    private MaterialIssueAddVM NewIssue(params (int InventoryId, decimal Quantity)[] items)
    {
        return new MaterialIssueAddVM
        {
            IssueDate = new DateTime(2026, 10, 1),
            PurposeId = _basics.PurposeId,
            LocationId = _basics.LocationId,
            ReceiverStaffId = _basics.StaffId,
            Items = items.Select(x => new MaterialIssueAddItemVM { InventoryId = x.InventoryId, Quantity = x.Quantity }).ToList()
        };
    }

    private static ApprovalVM Vm(string no) => new ApprovalVM { TableId = no };

    private async Task<string> AddDraftAsync(params (int InventoryId, decimal Quantity)[] items)
    {
        var (service, _) = CreateService();
        var result = await service.Add(NewIssue(items));
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return result.Data!.No;
    }

    private async Task<string> AddSubmittedAsync(params (int InventoryId, decimal Quantity)[] items)
    {
        var no = await AddDraftAsync(items);
        var (service, _) = CreateService();
        var send = await service.SendApproval(Vm(no));
        Assert.True(send.IsSuccess, send.ErrorMessage);
        return no;
    }

    private t_4030MaterialIssue LoadIssue(string no)
    {
        using var db = _testDb.CreateContext();
        return db.t_4030MaterialIssue.Include(x => x.t_4031MaterialIssueDetail).Single(x => x.No == no);
    }

    private decimal StockOf(int inventoryId)
    {
        using var db = _testDb.CreateContext();
        return db.t_4000Inventory.Single(x => x.Id == inventoryId).Quantity ?? 0m;
    }

    private void SetStock(int inventoryId, decimal quantity)
    {
        using var db = _testDb.CreateContext();
        db.t_4000Inventory.Single(x => x.Id == inventoryId).Quantity = quantity;
        db.SaveChanges();
    }

    private List<t_4040StockMovement> Movements()
    {
        using var db = _testDb.CreateContext();
        return db.t_4040StockMovement.OrderBy(x => x.Id).ToList();
    }

    // ---------- 建立 ----------

    [Fact]
    public async Task Add_建立草稿並帶入單號與明細快照()
    {
        var (service, _) = CreateService();

        var result = await service.Add(NewIssue((_inventoryA.Id, 3m)));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.StartsWith("IS", result.Data!.No);
        Assert.Empty(result.Data.Warnings);

        var issue = LoadIssue(result.Data.No);
        Assert.Equal((int)MaterialIssueStatus.草稿, issue.Status);
        Assert.Equal(7, issue.CreateUserId);
        var detail = Assert.Single(issue.t_4031MaterialIssueDetail);
        Assert.Equal(3m, detail.Quantity);
        Assert.Equal("A001", detail.No);
        Assert.Equal("品項A001", detail.Name);
        Assert.Equal("個", detail.Unit);
    }

    [Fact]
    public async Task Add_庫存不足仍可存草稿但回傳警告()
    {
        var (service, _) = CreateService();

        var result = await service.Add(NewIssue((_inventoryA.Id, 99m)));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var warning = Assert.Single(result.Data!.Warnings);
        Assert.Contains("A001", warning);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
    }

    [Fact]
    public async Task Add_沒有明細被拒絕()
    {
        var (service, _) = CreateService();
        var result = await service.Add(NewIssue());
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.IncompleteInfo, result.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Add_數量必須大於零(int quantity)
    {
        var (service, _) = CreateService();
        var result = await service.Add(NewIssue((_inventoryA.Id, quantity)));
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_同一庫存項目重複被拒絕()
    {
        var (service, _) = CreateService();
        var result = await service.Add(NewIssue((_inventoryA.Id, 1m), (_inventoryA.Id, 2m)));
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_庫存項目不在所選庫位被拒絕()
    {
        int otherLocationItemId;
        using (var db = _testDb.CreateContext())
        {
            otherLocationItemId = SeedData.CreateInventory(db, _basics, "C001", 5m, _basics.OtherLocationId).Id;
        }

        var (service, _) = CreateService();
        var result = await service.Add(NewIssue((otherLocationItemId, 1m)));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_領料人已離職被拒絕()
    {
        using (var db = _testDb.CreateContext())
        {
            db.t_1000Staff.Single(x => x.StaffId == _basics.StaffId).ResignationDate = new DateTime(2026, 1, 1);
            db.SaveChanges();
        }

        var (service, _) = CreateService();
        var result = await service.Add(NewIssue((_inventoryA.Id, 1m)));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    // ---------- 編輯、刪除 ----------

    [Fact]
    public async Task Edit_草稿可編輯並替換明細()
    {
        var no = await AddDraftAsync((_inventoryA.Id, 1m));
        var issue = LoadIssue(no);

        var vm = NewIssue((_inventoryB.Id, 2m));
        vm.Id = issue.Id;
        var (service, _) = CreateService();
        var result = await service.Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var detail = Assert.Single(LoadIssue(no).t_4031MaterialIssueDetail);
        Assert.Equal(_inventoryB.Id, detail.InventoryId);
        Assert.Equal(2m, detail.Quantity);
    }

    [Fact]
    public async Task Edit_簽核中不可編輯()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 1m));
        var vm = NewIssue((_inventoryA.Id, 2m));
        vm.Id = LoadIssue(no).Id;

        var (service, _) = CreateService();
        var result = await service.Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_已退回可編輯並可重新送簽()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 1m));
        var (service, _) = CreateService();
        Assert.True((await service.RejectApproval(Vm(no))).IsSuccess);

        var vm = NewIssue((_inventoryA.Id, 4m));
        vm.Id = LoadIssue(no).Id;
        var (service2, _) = CreateService();
        Assert.True((await service2.Edit(vm)).IsSuccess);

        var (service3, _) = CreateService();
        Assert.True((await service3.SendApproval(Vm(no))).IsSuccess);
        Assert.Equal((int)MaterialIssueStatus.簽核中, LoadIssue(no).Status);
    }

    [Fact]
    public async Task Delete_草稿可刪除_其他狀態不可()
    {
        var draftNo = await AddDraftAsync((_inventoryA.Id, 1m));
        var submittedNo = await AddSubmittedAsync((_inventoryB.Id, 1m));

        var (service, _) = CreateService();
        var submittedResult = await service.Delete(LoadIssue(submittedNo).Id);
        Assert.False(submittedResult.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, submittedResult.ErrorCode);

        var (service2, _) = CreateService();
        var draftResult = await service2.Delete(LoadIssue(draftNo).Id);
        Assert.True(draftResult.IsSuccess, draftResult.ErrorMessage);

        using var db = _testDb.CreateContext();
        Assert.False(db.t_4030MaterialIssue.Any(x => x.No == draftNo));
        Assert.Empty(db.t_4031MaterialIssueDetail.Where(x => x.Name == "品項A001"));
    }

    // ---------- 送簽 ----------

    [Fact]
    public async Task SendApproval_庫存不足被拒絕且狀態維持草稿()
    {
        var no = await AddDraftAsync((_inventoryA.Id, 99m));

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InsufficientInventory, result.ErrorCode);
        Assert.Contains("A001", result.ErrorMessage);
        Assert.Equal((int)MaterialIssueStatus.草稿, LoadIssue(no).Status);
    }

    [Fact]
    public async Task SendApproval_引擎失敗時狀態維持草稿()
    {
        var no = await AddDraftAsync((_inventoryA.Id, 1m));
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "找不到簽核流程");

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)MaterialIssueStatus.草稿, LoadIssue(no).Status);
    }

    [Fact]
    public async Task SendApproval_簽核中不可重複送簽()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 1m));

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
    }

    // ---------- 核准與扣庫存 ----------

    [Fact]
    public async Task Approval_非最後一關不扣庫存()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));
        _approval.ApprovalResult = ResultModel<string>.Ok("已核准，等待下一關");

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
        Assert.Equal((int)MaterialIssueStatus.簽核中, LoadIssue(no).Status);
        Assert.Empty(Movements());
    }

    [Fact]
    public async Task Approval_最後一關核准才扣庫存並寫入異動紀錄()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m), (_inventoryB.Id, 5m));
        // 送簽後庫存不變
        Assert.Equal(10m, StockOf(_inventoryA.Id));

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(7m, StockOf(_inventoryA.Id));
        Assert.Equal(0m, StockOf(_inventoryB.Id));
        Assert.Equal((int)MaterialIssueStatus.已核准, LoadIssue(no).Status);

        var movements = Movements();
        Assert.Equal(2, movements.Count);
        var a = movements.Single(x => x.InventoryId == _inventoryA.Id);
        Assert.Equal((int)StockMovementType.領料出庫, a.MovementType);
        Assert.Equal((int)TableType.領料單, a.SourceTableType);
        Assert.Equal(no, a.SourceNo);
        Assert.Equal(-3m, a.Quantity);
        Assert.Equal(10m, a.BeforeQuantity);
        Assert.Equal(7m, a.AfterQuantity);
        Assert.Equal(7, a.OperatorUserId);
    }

    [Fact]
    public async Task Approval_最後一關庫存不足_整筆回滾且狀態維持簽核中()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 8m));
        SetStock(_inventoryA.Id, 2m); // 送簽之後庫存被別張單據用掉

        var (service, db) = CreateService();
        // 模擬簽核引擎自己寫入一筆紀錄，驗證失敗時連它一起回滾
        _approval.OnApproval = async () =>
        {
            db.SystemConfig.Add(new SystemConfig { ConfigType = "EngineMarker", Name = "引擎紀錄", Sort = 0, IsActive = true });
            await db.SaveChangesAsync();
        };

        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InsufficientInventory, result.ErrorCode);
        Assert.Equal((int)MaterialIssueStatus.簽核中, LoadIssue(no).Status);
        Assert.Equal(2m, StockOf(_inventoryA.Id));
        Assert.Empty(Movements());

        using var check = _testDb.CreateContext();
        Assert.False(check.SystemConfig.Any(x => x.ConfigType == "EngineMarker"));
    }

    [Fact]
    public async Task Approval_庫存被並行異動_回傳友善訊息且不扣庫存()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));

        var (service, db) = CreateService();
        // 讀取庫存之後、儲存之前，別人改了這筆庫存（RowVersion 變了）
        db.BeforeNextSave = c => c.Database.ExecuteSqlRaw("UPDATE t_4000Inventory SET RowVersion = x'FF'");

        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.SpecificRecordChanged, result.ErrorCode);
        Assert.Equal("庫存已被異動，請重新操作", result.ErrorMessage);
        Assert.Equal((int)MaterialIssueStatus.簽核中, LoadIssue(no).Status);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
        Assert.Empty(Movements());
    }

    [Fact]
    public async Task Approval_草稿狀態不可簽核()
    {
        var no = await AddDraftAsync((_inventoryA.Id, 1m));

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
    }

    [Fact]
    public async Task Approval_單號不存在回傳找不到資料()
    {
        var (service, _) = CreateService();
        var result = await service.Approval(Vm("IS0000000"));
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 退回、撤銷、作廢 ----------

    [Fact]
    public async Task RejectApproval_退回後狀態為已退回且不動庫存()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));

        var (service, _) = CreateService();
        var result = await service.RejectApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)MaterialIssueStatus.已退回, LoadIssue(no).Status);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
    }

    [Fact]
    public async Task RevokeApproval_撤銷後狀態為已退回()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));

        var (service, _) = CreateService();
        var result = await service.RevokeApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)MaterialIssueStatus.已退回, LoadIssue(no).Status);
    }

    [Fact]
    public async Task VoidApproval_已核准可作廢並回補庫存()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));
        var (approveService, _) = CreateService();
        Assert.True((await approveService.Approval(Vm(no))).IsSuccess);
        Assert.Equal(7m, StockOf(_inventoryA.Id));

        var (service, _) = CreateService();
        var result = await service.VoidApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
        Assert.Equal((int)MaterialIssueStatus.已作廢, LoadIssue(no).Status);

        var movements = Movements();
        Assert.Equal(2, movements.Count);
        var voided = movements.Last();
        Assert.Equal((int)StockMovementType.領料作廢, voided.MovementType);
        Assert.Equal(3m, voided.Quantity);
        Assert.Equal(7m, voided.BeforeQuantity);
        Assert.Equal(10m, voided.AfterQuantity);
    }

    [Fact]
    public async Task VoidApproval_未核准不可作廢()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));

        var (service, _) = CreateService();
        var result = await service.VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
    }

    [Fact]
    public async Task VoidApproval_已作廢為終態不可再次作廢()
    {
        var no = await AddSubmittedAsync((_inventoryA.Id, 3m));
        var (s1, _) = CreateService();
        Assert.True((await s1.Approval(Vm(no))).IsSuccess);
        var (s2, _) = CreateService();
        Assert.True((await s2.VoidApproval(Vm(no))).IsSuccess);

        var (s3, _) = CreateService();
        var again = await s3.VoidApproval(Vm(no));

        Assert.False(again.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, again.ErrorCode);
        Assert.Equal(10m, StockOf(_inventoryA.Id));
    }
}
