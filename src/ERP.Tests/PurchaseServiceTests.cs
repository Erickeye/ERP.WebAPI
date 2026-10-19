using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.ViewModels;
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
}
