using ERP.EntityModels.Models;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Xunit;

namespace ERP.Tests;

public class StockMovementRecorderTests : IDisposable
{
    private readonly TestDb _testDb = new();

    public void Dispose() => _testDb.Dispose();

    [Fact]
    public void Apply_既有庫存_記錄異動前後數量()
    {
        using var db = _testDb.CreateContext();
        var basics = SeedData.CreateBasics(db);
        var inventory = SeedData.CreateInventory(db, basics, "A001", 10m);

        new StockMovementRecorder(db).Apply(
            inventory, StockMovementType.領料出庫, TableType.領料單, "IS1", -4m, 9, new DateTime(2026, 10, 2));
        db.SaveChanges();

        using var check = _testDb.CreateContext();
        Assert.Equal(6m, check.t_4000Inventory.Single(x => x.Id == inventory.Id).Quantity);
        var movement = Assert.Single(check.t_4040StockMovement);
        Assert.Equal(10m, movement.BeforeQuantity);
        Assert.Equal(6m, movement.AfterQuantity);
        Assert.Equal(-4m, movement.Quantity);
        Assert.Equal(9, movement.OperatorUserId);
        Assert.Equal(new DateTime(2026, 10, 2), movement.MovementTime);
    }

    [Fact]
    public void Apply_新庫存項目_異動紀錄會掛到新項目()
    {
        using var db = _testDb.CreateContext();
        var basics = SeedData.CreateBasics(db);
        var inventory = new t_4000Inventory
        {
            SupplierId = basics.SupplierId,
            Name = "新品項",
            LocationId = basics.LocationId,
            No = "N001",
            Quantity = 0m
        };
        db.t_4000Inventory.Add(inventory);

        new StockMovementRecorder(db).Apply(
            inventory, StockMovementType.進貨入庫, TableType.進貨單, "PO1", 5m, 1, DateTime.Now);
        db.SaveChanges();

        using var check = _testDb.CreateContext();
        var saved = check.t_4000Inventory.Single(x => x.No == "N001");
        Assert.Equal(5m, saved.Quantity);
        var movement = Assert.Single(check.t_4040StockMovement);
        Assert.Equal(saved.Id, movement.InventoryId);
        Assert.Equal(0m, movement.BeforeQuantity);
        Assert.Equal(5m, movement.AfterQuantity);
    }
}
