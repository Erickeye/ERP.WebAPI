using System.ComponentModel.DataAnnotations;
using ERP.EntityModels.Models;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// code review 後修正的行為：用途下拉只列啟用項目、結束日包含整天、數量範圍驗證。
/// </summary>
public class ReviewFixTests : IDisposable
{
    private readonly TestDb _testDb = new();

    public void Dispose() => _testDb.Dispose();

    // 異動查詢會關聯操作人，需要先有使用者資料（Id = 1）
    private void SeedMovement()
    {
        using var db = _testDb.CreateContext();
        db.User.Add(new User
        {
            RoleId = 1,
            CreateDate = new DateTime(2026, 1, 1),
            Name = "操作人",
            Account = "operator",
            Pwd = "placeholder"
        });
        var basics = SeedData.CreateBasics(db);
        var inventory = SeedData.CreateInventory(db, basics, "A001", 10m);
        new StockMovementRecorder(db).Apply(
            inventory, StockMovementType.領料出庫, TableType.領料單, "IS1151001", -1m, 1,
            new DateTime(2026, 10, 5, 15, 30, 0));
        db.SaveChanges();
    }

    [Fact]
    public async Task 領料用途下拉_不列出已停用的項目()
    {
        using var db = _testDb.CreateContext();
        db.SystemConfig.AddRange(
            new SystemConfig { ConfigType = "MaterialIssuePurpose", Name = "啟用中", Sort = 1, IsActive = true },
            new SystemConfig { ConfigType = "MaterialIssuePurpose", Name = "已停用", Sort = 2, IsActive = false });
        db.SaveChanges();

        var result = await new SharedService(db).GetMaterialIssuePurposeSelect();

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var item = Assert.Single(result.Data!.Items);
        Assert.Equal("啟用中", item.Text);
    }

    [Fact]
    public async Task 異動查詢_結束日只給日期時包含當天整天()
    {
        SeedMovement();

        using var queryDb = _testDb.CreateContext();
        var vm = new StockMovementSearchVM { MovementTimeEnd = new DateTime(2026, 10, 5) };
        var result = await new _4040StockMovementService(queryDb).Index(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Single(result.Data!.Items);
    }

    [Fact]
    public async Task 異動查詢_結束日早於異動日期時查不到()
    {
        SeedMovement();

        using var queryDb = _testDb.CreateContext();
        var vm = new StockMovementSearchVM { MovementTimeEnd = new DateTime(2026, 10, 4) };
        var result = await new _4040StockMovementService(queryDb).Index(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Empty(result.Data!.Items);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(0.01, true)]
    [InlineData(5, true)]
    public void 領料數量_DataAnnotations驗證必須大於零(double quantity, bool expectedValid)
    {
        var item = new MaterialIssueAddItemVM { InventoryId = 1, Quantity = (decimal)quantity };

        var isValid = Validator.TryValidateObject(item, new ValidationContext(item), new List<ValidationResult>(), true);

        Assert.Equal(expectedValid, isValid);
    }
}
