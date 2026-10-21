using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 進貨單回歸測試：核准入庫與作廢現在會寫入庫存異動紀錄，並處理並行衝突。
/// </summary>
public class PurchaseServiceTests : IDisposable
{
    private readonly TestDb _testDb = new();
    private readonly FakeApprovalWorkflowService _approval = new();
    private readonly SeedData.Basics _basics;

    public PurchaseServiceTests()
    {
        using var db = _testDb.CreateContext();
        _basics = SeedData.CreateBasics(db);
    }

    public void Dispose() => _testDb.Dispose();

    private (I_4010PurchaseService Service, TestERPDbContext Db) CreateService()
    {
        var db = _testDb.CreateContext();
        var service = new _4010PurchaseService(
            db,
            new FakeCurrentUser { UserId = 5 },
            new FakeSerialService(),
            _approval,
            new StockMovementRecorder(db));
        return (service, db);
    }

    private string AddPurchase(string itemNo, decimal quantity)
    {
        using var db = _testDb.CreateContext();
        var purchase = new t_4010Purchase
        {
            No = "PO1151001",
            SupplierId = _basics.SupplierId,
            LocationId = _basics.LocationId,
            ProjectName = "測試專案",
            PurchaseDate = new DateTime(2026, 10, 1),
            PaymentMethodId = 1,
            Payer = "tester",
            CreateTime = DateTime.Now,
            CreateUserId = 5
        };
        purchase.t_4011PurchaseDetail.Add(new t_4011PurchaseDetail
        {
            No = itemNo,
            Name = $"品項{itemNo}",
            Unit = "個",
            Quantity = quantity,
            Price = 10m
        });
        db.t_4010Purchase.Add(purchase);
        db.SaveChanges();
        return purchase.No;
    }

    private static ApprovalVM Vm(string no) => new ApprovalVM { TableId = no };

    [Fact]
    public async Task Approval_既有庫存入庫並寫入進貨入庫異動()
    {
        int inventoryId;
        using (var db = _testDb.CreateContext())
        {
            inventoryId = SeedData.CreateInventory(db, _basics, "A001", 10m).Id;
        }
        var no = AddPurchase("A001", 4m);

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        Assert.Equal(14m, check.t_4000Inventory.Single(x => x.Id == inventoryId).Quantity);
        var movement = Assert.Single(check.t_4040StockMovement);
        Assert.Equal((int)StockMovementType.進貨入庫, movement.MovementType);
        Assert.Equal((int)TableType.進貨單, movement.SourceTableType);
        Assert.Equal(no, movement.SourceNo);
        Assert.Equal(4m, movement.Quantity);
        Assert.Equal(10m, movement.BeforeQuantity);
        Assert.Equal(14m, movement.AfterQuantity);
        Assert.Equal(5, movement.OperatorUserId);
    }

    [Fact]
    public async Task Approval_庫存不存在時新增庫存項目且異動前為零()
    {
        var no = AddPurchase("N001", 6m);

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        var inventory = check.t_4000Inventory.Single(x => x.No == "N001");
        Assert.Equal(6m, inventory.Quantity);
        var movement = Assert.Single(check.t_4040StockMovement);
        Assert.Equal(inventory.Id, movement.InventoryId);
        Assert.Equal(0m, movement.BeforeQuantity);
        Assert.Equal(6m, movement.AfterQuantity);
    }

    [Fact]
    public async Task Approval_非最後一關不入庫()
    {
        var no = AddPurchase("N001", 6m);
        _approval.ApprovalResult = ResultModel<string>.Ok("已核准，等待下一關");

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        Assert.Empty(check.t_4000Inventory);
        Assert.Empty(check.t_4040StockMovement);
    }

    [Fact]
    public async Task Approval_庫存被並行異動_回傳友善訊息()
    {
        using (var db = _testDb.CreateContext())
        {
            SeedData.CreateInventory(db, _basics, "A001", 10m);
        }
        var no = AddPurchase("A001", 4m);

        var (service, db2) = CreateService();
        db2.BeforeNextSave = c => c.Database.ExecuteSqlRaw("UPDATE t_4000Inventory SET RowVersion = x'FF'");

        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.SpecificRecordChanged, result.ErrorCode);
        Assert.Equal("庫存已被異動，請重新操作", result.ErrorMessage);
    }

    [Fact]
    public async Task VoidApproval_作廢扣回庫存並寫入進貨作廢異動()
    {
        int inventoryId;
        using (var db = _testDb.CreateContext())
        {
            inventoryId = SeedData.CreateInventory(db, _basics, "A001", 10m).Id;
        }
        var no = AddPurchase("A001", 4m);

        var (service, _) = CreateService();
        var result = await service.VoidApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        Assert.Equal(6m, check.t_4000Inventory.Single(x => x.Id == inventoryId).Quantity);
        var movement = Assert.Single(check.t_4040StockMovement);
        Assert.Equal((int)StockMovementType.進貨作廢, movement.MovementType);
        Assert.Equal(-4m, movement.Quantity);
        Assert.Equal(10m, movement.BeforeQuantity);
        Assert.Equal(6m, movement.AfterQuantity);
    }

    [Fact]
    public async Task VoidApproval_庫存不足以扣回時被拒絕且不異動()
    {
        int inventoryId;
        using (var db = _testDb.CreateContext())
        {
            inventoryId = SeedData.CreateInventory(db, _basics, "A001", 2m).Id;
        }
        var no = AddPurchase("A001", 4m);

        var (service, _) = CreateService();
        var result = await service.VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidApproval, result.ErrorCode);
        using var check = _testDb.CreateContext();
        Assert.Equal(2m, check.t_4000Inventory.Single(x => x.Id == inventoryId).Quantity);
        Assert.Empty(check.t_4040StockMovement);
    }

    // ---------- 供應商串接（供應商管理 ticket 02） ----------

    private PurchaseAddVM ValidAddVm(int supplierId) => new()
    {
        SupplierId = supplierId,
        LocationId = _basics.LocationId,
        ProjectName = "測試專案",
        PurchaseDate = new DateTime(2026, 10, 1),
        PaymentMethodId = 1,
        Payer = "tester",
        Items =
        {
            new PurchaseItemVM { No = "A001", Name = "品項A001", Unit = "個", Quantity = 2m, Price = 10m }
        }
    };

    private int DisableSupplier(int supplierId)
    {
        using var db = _testDb.CreateContext();
        db.t_4060Supplier.Single(x => x.Id == supplierId).IsActive = false;
        db.SaveChanges();
        return supplierId;
    }

    [Fact]
    public async Task Add_供應商不存在被擋下且不建立進貨單()
    {
        var (service, db) = CreateService();

        var result = await service.Add(ValidAddVm(supplierId: 999));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("供應商不存在", result.ErrorMessage);
        Assert.Empty(db.t_4010Purchase);
    }

    [Fact]
    public async Task Add_供應商存在時可建立進貨單()
    {
        var (service, db) = CreateService();

        var result = await service.Add(ValidAddVm(_basics.SupplierId));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Single(db.t_4010Purchase);
    }

    [Fact]
    public async Task Add_供應商已停用仍可儲存草稿_只在送簽時才擋()
    {
        DisableSupplier(_basics.SupplierId);
        var (service, db) = CreateService();

        var result = await service.Add(ValidAddVm(_basics.SupplierId));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Single(db.t_4010Purchase);
    }

    [Fact]
    public async Task Edit_改成不存在的供應商被擋下且原供應商不變()
    {
        var no = AddPurchase("A001", 4m);
        int id;
        using (var db = _testDb.CreateContext())
        {
            id = db.t_4010Purchase.Single(x => x.No == no).Id;
        }
        var vm = ValidAddVm(supplierId: 999);
        vm.Id = id;

        var (service, _) = CreateService();
        var result = await service.Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("供應商不存在", result.ErrorMessage);
        using var check = _testDb.CreateContext();
        Assert.Equal(_basics.SupplierId, check.t_4010Purchase.Single(x => x.Id == id).SupplierId);
    }

    [Fact]
    public async Task SendApproval_供應商啟用中可送簽()
    {
        var no = AddPurchase("A001", 4m);
        var sent = false;
        _approval.OnSend = () =>
        {
            sent = true;
            return Task.CompletedTask;
        };

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.True(sent);
    }

    [Fact]
    public async Task SendApproval_供應商已停用被擋下且不進簽核引擎()
    {
        var no = AddPurchase("A001", 4m);
        DisableSupplier(_basics.SupplierId);
        var sent = false;
        _approval.OnSend = () =>
        {
            sent = true;
            return Task.CompletedTask;
        };

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidRequest, result.ErrorCode);
        Assert.Contains("停用", result.ErrorMessage);
        Assert.False(sent);
    }

    [Fact]
    public async Task SendApproval_改選啟用中供應商後可再送簽()
    {
        var no = AddPurchase("A001", 4m);
        DisableSupplier(_basics.SupplierId);
        int otherSupplierId;
        using (var db = _testDb.CreateContext())
        {
            otherSupplierId = SeedData.CreateSupplier(db, "S002", "另一間供應商").Id;
            db.t_4010Purchase.Single(x => x.No == no).SupplierId = otherSupplierId;
            db.SaveChanges();
        }

        var (service, _) = CreateService();
        var result = await service.SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Approval_送簽後供應商被停用_仍照常核准並增加該供應商庫存()
    {
        int inventoryId;
        using (var db = _testDb.CreateContext())
        {
            inventoryId = SeedData.CreateInventory(db, _basics, "A001", 10m).Id;
        }
        var no = AddPurchase("A001", 4m);
        DisableSupplier(_basics.SupplierId);

        var (service, _) = CreateService();
        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        var inventory = check.t_4000Inventory.Single(x => x.Id == inventoryId);
        Assert.Equal(14m, inventory.Quantity);
        Assert.Equal(_basics.SupplierId, inventory.SupplierId);
    }
}
