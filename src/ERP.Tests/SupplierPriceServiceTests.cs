using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 供應商報價服務的行為測試：新增、編輯、欄位驗證、（供應商＋料號）唯一、刪除、列表與詳情。
/// </summary>
public class SupplierPriceServiceTests : IDisposable
{
    private readonly TestDb _testDb = new();

    public void Dispose() => _testDb.Dispose();

    // ---------- 測試輔助 ----------

    private _4061SupplierPriceService CreateService(TestERPDbContext db, int userId = 1) =>
        new(db, new FakeCurrentUser { UserId = userId });

    private int SeedSupplier(string no = "S001", string name = "甲供應商", bool isActive = true)
    {
        using var db = _testDb.CreateContext();
        var entity = new t_4060Supplier { No = no, Name = name, IsActive = true };
        db.t_4060Supplier.Add(entity);
        db.SaveChanges();

        // IsActive 的資料表預設值是 true，EF 會把 false 視為「未指定」，所以停用的供應商要先存成啟用再改
        if (!isActive)
        {
            entity.IsActive = false;
            db.SaveChanges();
        }
        return entity.Id;
    }

    private static SupplierPriceAddVM ValidInput(int supplierId, string no = "P001", decimal price = 12.5m) => new()
    {
        SupplierId = supplierId,
        No = no,
        Name = "螺絲",
        Unit = "個",
        Price = price
    };

    private t_4061SupplierPrice LoadPrice(int id)
    {
        using var db = _testDb.CreateContext();
        return db.t_4061SupplierPrice.AsNoTracking().Single(x => x.Id == id);
    }

    // ---------- 新增 ----------

    [Fact]
    public async Task Add_有效輸入_存入報價並記錄建立與修改資訊()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var before = DateTime.Now.AddSeconds(-1);

        var result = await CreateService(db, userId: 7).Add(ValidInput(supplierId));

        Assert.True(result.IsSuccess);
        var saved = LoadPrice(result.Data);
        Assert.Equal(supplierId, saved.SupplierId);
        Assert.Equal("P001", saved.No);
        Assert.Equal("螺絲", saved.Name);
        Assert.Equal("個", saved.Unit);
        Assert.Equal(12.5m, saved.Price);
        Assert.Equal(7, saved.CreateUserId);
        Assert.Equal(7, saved.UpdateUserId);
        Assert.True(saved.CreateTime >= before);
        Assert.Equal(saved.CreateTime, saved.UpdateTime);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Add_料號未填_被擋下(string? no)
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId);
        input.No = no;

        var result = await CreateService(db).Add(input);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("料號", result.ErrorMessage);
        Assert.Empty(db.t_4061SupplierPrice);
    }

    [Fact]
    public async Task Add_品名與單位未填_被擋下()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId);
        input.Name = null;
        input.Unit = " ";

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("品名", result.ErrorMessage);
        Assert.Contains("單位", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_欄位超過長度限制_被擋下()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId, no: new string('A', 65));
        input.Name = new string('N', 129);
        input.Unit = new string('U', 17);
        input.Note = new string('B', 257);

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("料號", result.ErrorMessage);
        Assert.Contains("品名", result.ErrorMessage);
        Assert.Contains("單位", result.ErrorMessage);
        Assert.Contains("備註", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_欄位剛好等於長度上限_可以新增()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId, no: new string('A', 64));
        input.Name = new string('N', 128);
        input.Unit = new string('U', 16);
        input.Note = new string('B', 256);

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(0.001)]
    public async Task Add_單價未填或不大於零_被擋下(double? price)
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId);
        input.Price = price.HasValue ? (decimal)price.Value : null;

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("單價", result.ErrorMessage);
        Assert.Empty(db.t_4061SupplierPrice);
    }

    [Fact]
    public async Task Add_單價超過兩位小數_被擋下()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(supplierId, price: 1.234m));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("單價", result.ErrorMessage);
        Assert.Empty(db.t_4061SupplierPrice);
    }

    [Fact]
    public async Task Add_單價最小值與兩位小數_可以新增()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(supplierId, price: 0.01m));

        Assert.True(result.IsSuccess);
        Assert.Equal(0.01m, LoadPrice(result.Data).Price);
    }

    [Fact]
    public async Task Add_字串前後空白_去除後儲存_備註空白視為未填()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId, no: "  P001  ");
        input.Name = " 螺絲 ";
        input.Note = "   ";

        var result = await CreateService(db).Add(input);

        var saved = LoadPrice(result.Data);
        Assert.Equal("P001", saved.No);
        Assert.Equal("螺絲", saved.Name);
        Assert.Null(saved.Note);
    }

    [Fact]
    public async Task Add_供應商不存在_回找不到供應商()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(supplierId: 999));

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Contains("供應商", result.ErrorMessage);
        Assert.Empty(db.t_4061SupplierPrice);
    }

    [Fact]
    public async Task Add_供應商已停用_仍可新增報價()
    {
        var supplierId = SeedSupplier(isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(supplierId));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Add_同供應商料號重複_被擋下並提示()
    {
        var supplierId = SeedSupplier();
        using (var db = _testDb.CreateContext())
        {
            await CreateService(db).Add(ValidInput(supplierId, no: "P001"));
        }
        using var db2 = _testDb.CreateContext();

        var result = await CreateService(db2).Add(ValidInput(supplierId, no: "P001"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("料號", result.ErrorMessage);
        Assert.Equal(1, db2.t_4061SupplierPrice.Count());
    }

    [Fact]
    public async Task Add_不同供應商可有相同料號_同供應商品名可重複()
    {
        var supplierA = SeedSupplier("S001", "甲");
        var supplierB = SeedSupplier("S002", "乙");
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var a = await service.Add(ValidInput(supplierA, no: "P001"));
        var b = await service.Add(ValidInput(supplierB, no: "P001"));
        var sameName = await service.Add(ValidInput(supplierA, no: "P002"));

        Assert.True(a.IsSuccess);
        Assert.True(b.IsSuccess);
        Assert.True(sameName.IsSuccess);
    }

    // ---------- 編輯 ----------

    private async Task<int> AddPrice(int supplierId, string no = "P001", decimal price = 12.5m, int userId = 1)
    {
        using var db = _testDb.CreateContext();
        var result = await CreateService(db, userId).Add(ValidInput(supplierId, no, price));
        Assert.True(result.IsSuccess);
        return result.Data;
    }

    private void DisableSupplier(int supplierId)
    {
        using var db = _testDb.CreateContext();
        db.t_4060Supplier.Single(x => x.Id == supplierId).IsActive = false;
        db.SaveChanges();
    }

    private static SupplierPriceAddVM ValidInputWithId(int id, int supplierId, string no, decimal price = 12.5m)
    {
        var input = ValidInput(supplierId, no, price);
        input.Id = id;
        return input;
    }

    [Fact]
    public async Task Edit_改價與其他欄位_直接覆蓋並更新修改資訊_建立資訊不變()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, userId: 7);
        var created = LoadPrice(id);
        await Task.Delay(20);
        using var db = _testDb.CreateContext();
        var input = new SupplierPriceAddVM
        {
            Id = id, SupplierId = supplierId, No = "P001", Name = "六角螺絲", Unit = "盒", Price = 99.99m, Note = "議價"
        };

        var result = await CreateService(db, userId: 9).Edit(input);

        Assert.True(result.IsSuccess);
        var saved = LoadPrice(id);
        Assert.Equal("六角螺絲", saved.Name);
        Assert.Equal("盒", saved.Unit);
        Assert.Equal(99.99m, saved.Price);
        Assert.Equal("議價", saved.Note);
        Assert.Equal(7, saved.CreateUserId);
        Assert.Equal(created.CreateTime, saved.CreateTime);
        Assert.Equal(9, saved.UpdateUserId);
        Assert.True(saved.UpdateTime > created.UpdateTime);
        using var check = _testDb.CreateContext();
        Assert.Equal(1, check.t_4061SupplierPrice.Count());
    }

    [Fact]
    public async Task Edit_修改料號_可以改成未使用的料號()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, no: "P001");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierId, "P999"));

        Assert.True(result.IsSuccess);
        Assert.Equal("P999", LoadPrice(id).No);
    }

    [Fact]
    public async Task Edit_料號沿用自己的原料號_不算重複()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, no: "P001");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierId, "P001"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Edit_料號改成同供應商其他報價的料號_被擋下_原資料不變()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, no: "P001");
        await AddPrice(supplierId, no: "P002");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierId, "P002"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("料號", result.ErrorMessage);
        Assert.Equal("P001", LoadPrice(id).No);
    }

    [Fact]
    public async Task Edit_料號與其他供應商的料號相同_可以()
    {
        var supplierA = SeedSupplier("S001", "甲");
        var supplierB = SeedSupplier("S002", "乙");
        var id = await AddPrice(supplierA, no: "P001");
        await AddPrice(supplierB, no: "P002");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierA, "P002"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Edit_欄位不合規定_被擋下_原資料不變()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, price: 10m);
        using var db = _testDb.CreateContext();
        var input = ValidInputWithId(id, supplierId, "P001");
        input.Price = 0m;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Equal(10m, LoadPrice(id).Price);
    }

    [Fact]
    public async Task Edit_報價不存在_回找不到資料()
    {
        var supplierId = SeedSupplier();
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(999, supplierId, "P001"));

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_輸入的供應商與原報價不同_仍留在原供應商_報價不能換供應商()
    {
        var supplierA = SeedSupplier("S001", "甲");
        var supplierB = SeedSupplier("S002", "乙");
        var id = await AddPrice(supplierA);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierB, "P001"));

        Assert.True(result.IsSuccess);
        Assert.Equal(supplierA, LoadPrice(id).SupplierId);
    }

    [Fact]
    public async Task Edit_供應商已停用_仍可編輯報價()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId);
        DisableSupplier(supplierId);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Edit(ValidInputWithId(id, supplierId, "P001", 20m));

        Assert.True(result.IsSuccess);
        Assert.Equal(20m, LoadPrice(id).Price);
    }

    // ---------- 刪除 ----------

    [Fact]
    public async Task Delete_刪除報價_只刪該筆()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, no: "P001");
        var otherId = await AddPrice(supplierId, no: "P002");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess);
        using var check = _testDb.CreateContext();
        Assert.False(check.t_4061SupplierPrice.Any(x => x.Id == id));
        Assert.True(check.t_4061SupplierPrice.Any(x => x.Id == otherId));
        Assert.True(check.t_4060Supplier.Any(x => x.Id == supplierId));
    }

    [Fact]
    public async Task Delete_報價不存在_回找不到資料()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Delete_供應商已停用_仍可刪除報價()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId);
        DisableSupplier(supplierId);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess);
    }

    // ---------- 列表與詳情 ----------

    private async Task AddNamedPrice(int supplierId, string no, string name)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput(supplierId, no);
        input.Name = name;
        var result = await CreateService(db).Add(input);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Index_只列該供應商的報價_預設依料號排序()
    {
        var supplierA = SeedSupplier("S001", "甲");
        var supplierB = SeedSupplier("S002", "乙");
        await AddPrice(supplierA, no: "P003");
        await AddPrice(supplierA, no: "P001");
        await AddPrice(supplierA, no: "P002");
        await AddPrice(supplierB, no: "P000");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierPriceSearchVM { SupplierId = supplierA });

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { "P001", "P002", "P003" }, result.Data.Items.Select(x => x.No));
        Assert.Equal(3, result.Data.TotalCount);
    }

    [Fact]
    public async Task Index_列表欄位完整()
    {
        var supplierId = SeedSupplier();
        using (var db = _testDb.CreateContext())
        {
            var input = ValidInput(supplierId, "P001", 8.75m);
            input.Note = "議價";
            await CreateService(db).Add(input);
        }
        using var db2 = _testDb.CreateContext();

        var item = (await CreateService(db2).Index(new SupplierPriceSearchVM { SupplierId = supplierId })).Data.Items.Single();

        Assert.Equal(supplierId, item.SupplierId);
        Assert.Equal("P001", item.No);
        Assert.Equal("螺絲", item.Name);
        Assert.Equal("個", item.Unit);
        Assert.Equal(8.75m, item.Price);
        Assert.Equal("議價", item.Note);
        Assert.True(item.UpdateTime > DateTime.MinValue);
    }

    [Theory]
    [InlineData("A-100", 1)]
    [InlineData("100", 2)]
    [InlineData("螺絲", 2)]
    [InlineData("  螺絲  ", 2)]
    [InlineData("不存在", 0)]
    [InlineData("", 3)]
    [InlineData(null, 3)]
    public async Task Index_關鍵字比對料號與品名(string? keyword, int expectedCount)
    {
        var supplierId = SeedSupplier();
        await AddNamedPrice(supplierId, "A-100", "六角螺絲");
        await AddNamedPrice(supplierId, "B-100", "螺絲帽");
        await AddNamedPrice(supplierId, "C-200", "墊片");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierPriceSearchVM { SupplierId = supplierId, Keyword = keyword });

        Assert.Equal(expectedCount, result.Data.Items.Count);
    }

    [Fact]
    public async Task Index_分頁()
    {
        var supplierId = SeedSupplier();
        for (var i = 1; i <= 5; i++)
        {
            await AddPrice(supplierId, no: $"P00{i}");
        }
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierPriceSearchVM { SupplierId = supplierId, PageNumber = 2, PageSize = 2 });

        Assert.Equal(new[] { "P003", "P004" }, result.Data.Items.Select(x => x.No));
        Assert.Equal(5, result.Data.TotalCount);
        Assert.Equal(3, result.Data.TotalPages);
    }

    [Fact]
    public async Task Index_供應商不存在_回找不到供應商()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierPriceSearchVM { SupplierId = 999 });

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Contains("供應商", result.ErrorMessage);
    }

    [Fact]
    public async Task Index_供應商已停用_仍可列出報價_沒有報價回空清單()
    {
        var withPrice = SeedSupplier("S001", "甲");
        var empty = SeedSupplier("S002", "乙");
        await AddPrice(withPrice);
        DisableSupplier(withPrice);
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var a = await service.Index(new SupplierPriceSearchVM { SupplierId = withPrice });
        var b = await service.Index(new SupplierPriceSearchVM { SupplierId = empty });

        Assert.Single(a.Data.Items);
        Assert.True(b.IsSuccess);
        Assert.Empty(b.Data.Items);
    }

    [Fact]
    public async Task Get_取得單筆完整資料()
    {
        var supplierId = SeedSupplier();
        var id = await AddPrice(supplierId, no: "P001", price: 3.5m);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Get(id);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Data.Id);
        Assert.Equal(supplierId, result.Data.SupplierId);
        Assert.Equal("P001", result.Data.No);
        Assert.Equal(3.5m, result.Data.Price);
        Assert.True(result.Data.CreateTime > DateTime.MinValue);
    }

    [Fact]
    public async Task Get_報價不存在_回找不到資料()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Get(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }
}
