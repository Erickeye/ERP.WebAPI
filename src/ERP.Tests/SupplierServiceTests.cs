using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 供應商服務的行為測試：新增、編輯、欄位驗證、編號與統編唯一、停用與啟用、刪除、列表與詳情。
/// </summary>
public class SupplierServiceTests : IDisposable
{
    private readonly TestDb _testDb = new();

    public void Dispose() => _testDb.Dispose();

    // ---------- 測試輔助 ----------

    private _4060SupplierService CreateService(TestERPDbContext db) => new(db);

    private static SupplierAddVM ValidInput(string no = "S001", string name = "甲供應商") => new()
    {
        No = no,
        Name = name
    };

    /// <summary>
    /// 直接寫入一間供應商。IsActive 的資料表預設值是 true，EF 會把 false（CLR 預設值）視為「未指定」而改用預設值，
    /// 所以要建立停用的供應商時，先存成啟用再改成停用。
    /// </summary>
    private int SeedSupplier(string? no, string name, string? taxIdNumber = null, bool isActive = true)
    {
        using var db = _testDb.CreateContext();
        var entity = new t_4060Supplier
        {
            No = no,
            Name = name,
            TaxIdNumber = taxIdNumber,
            IsActive = true
        };
        db.t_4060Supplier.Add(entity);
        db.SaveChanges();

        if (!isActive)
        {
            entity.IsActive = false;
            db.SaveChanges();
        }
        return entity.Id;
    }

    private t_4060Supplier LoadSupplier(int id)
    {
        using var db = _testDb.CreateContext();
        return db.t_4060Supplier.AsNoTracking().Single(x => x.Id == id);
    }

    private void SeedPurchase(int supplierId)
    {
        using var db = _testDb.CreateContext();
        db.t_4010Purchase.Add(new t_4010Purchase
        {
            No = "PO1151001",
            SupplierId = supplierId,
            LocationId = 1,
            ProjectName = "測試專案",
            PurchaseDate = new DateTime(2026, 10, 1),
            PaymentMethodId = 1,
            Payer = "tester",
            CreateTime = DateTime.Now,
            CreateUserId = 5
        });
        db.SaveChanges();
    }

    private void SeedInventory(int supplierId)
    {
        using var db = _testDb.CreateContext();
        db.t_4000Inventory.Add(new t_4000Inventory
        {
            SupplierId = supplierId,
            Name = "品項",
            LocationId = 1,
            No = "A001",
            Unit = "個",
            Quantity = 1m
        });
        db.SaveChanges();
    }

    private void SeedPrice(int supplierId, string no)
    {
        using var db = _testDb.CreateContext();
        db.t_4061SupplierPrice.Add(new t_4061SupplierPrice
        {
            SupplierId = supplierId,
            No = no,
            Name = "螺絲",
            Unit = "個",
            Price = 1m,
            CreateTime = DateTime.Now,
            CreateUserId = 1,
            UpdateTime = DateTime.Now,
            UpdateUserId = 1
        });
        db.SaveChanges();
    }

    // ---------- 新增 ----------

    [Fact]
    public async Task Add_ValidInput_SavesActiveSupplierWithTrimmedValues()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.No = "  S001  ";
        input.Name = " 甲供應商 ";

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadSupplier(result.Data);
        Assert.Equal("S001", saved.No);
        Assert.Equal("甲供應商", saved.Name);
        Assert.True(saved.IsActive);
        Assert.Null(saved.TaxIdNumber);
    }

    [Fact]
    public async Task Add_WithOptionalFields_SavesThem()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.TaxIdNumber = "12345678";
        input.Owner = "王大明";
        input.ContactPhone = "02-12345678";
        input.FaxPhone = "02-87654321";
        input.Salesperson = "李小華";
        input.Address = "台北市測試路 1 號";
        input.BankName = "測試銀行";
        input.SubBankName = "測試分行";
        input.CheckingAccount = "111";
        input.RemittanceAccount = "222";
        input.PayDays = 30;
        input.CreditLine = 100000.5m;

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadSupplier(result.Data);
        Assert.Equal("12345678", saved.TaxIdNumber);
        Assert.Equal("王大明", saved.Owner);
        Assert.Equal("02-12345678", saved.ContactPhone);
        Assert.Equal("02-87654321", saved.FaxPhone);
        Assert.Equal("李小華", saved.Salesperson);
        Assert.Equal("台北市測試路 1 號", saved.Address);
        Assert.Equal("測試銀行", saved.BankName);
        Assert.Equal("測試分行", saved.SubBankName);
        Assert.Equal("111", saved.CheckingAccount);
        Assert.Equal("222", saved.RemittanceAccount);
        Assert.Equal(30, saved.PayDays);
        Assert.Equal(100000.5m, saved.CreditLine);
    }

    [Theory]
    [InlineData("No")]
    [InlineData("Name")]
    public async Task Add_MissingRequiredField_IsRejected(string field)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        typeof(SupplierAddVM).GetProperty(field)!.SetValue(input, "   ");

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Empty(db.t_4060Supplier);
    }

    [Theory]
    [InlineData("No", 65)]
    [InlineData("Name", 129)]
    [InlineData("Owner", 33)]
    [InlineData("ContactPhone", 33)]
    [InlineData("FaxPhone", 33)]
    [InlineData("Salesperson", 33)]
    [InlineData("Address", 129)]
    [InlineData("BankName", 65)]
    [InlineData("SubBankName", 65)]
    [InlineData("CheckingAccount", 65)]
    [InlineData("RemittanceAccount", 65)]
    public async Task Add_FieldTooLong_IsRejected(string field, int length)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        typeof(SupplierAddVM).GetProperty(field)!.SetValue(input, new string('甲', length));

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Empty(db.t_4060Supplier);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567A")]
    public async Task Add_InvalidTaxIdNumber_IsRejected(string taxIdNumber)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.TaxIdNumber = taxIdNumber;

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Empty(db.t_4060Supplier);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Add_BlankTaxIdNumber_IsTreatedAsNotFilled(string? taxIdNumber)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.TaxIdNumber = taxIdNumber;

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Null(LoadSupplier(result.Data).TaxIdNumber);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public async Task Add_PayDaysOutOfRange_IsRejected(int payDays)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.PayDays = payDays;

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_NegativeCreditLine_IsRejected()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.CreditLine = -1m;

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_DuplicateNo_IsRejected()
    {
        SeedSupplier("S001", "既有供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput("S001", "另一間"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("供應商編號", result.ErrorMessage);
        Assert.Equal(1, db.t_4060Supplier.Count());
    }

    [Fact]
    public async Task Add_DuplicateNoOfDisabledSupplier_IsRejected()
    {
        SeedSupplier("S001", "已停用供應商", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput("S001", "另一間"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_DuplicateTaxIdNumber_IsRejected()
    {
        SeedSupplier("S001", "既有供應商", taxIdNumber: "12345678");
        using var db = _testDb.CreateContext();
        var input = ValidInput("S002", "另一間");
        input.TaxIdNumber = "12345678";

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("統一編號", result.ErrorMessage);
        Assert.Equal(1, db.t_4060Supplier.Count());
    }

    [Fact]
    public async Task Add_DuplicateName_IsAllowed()
    {
        SeedSupplier("S001", "同名供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput("S002", "同名供應商"));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(2, db.t_4060Supplier.Count());
    }

    [Fact]
    public async Task Add_SeveralSuppliersWithoutTaxIdNumber_IsAllowed()
    {
        SeedSupplier("S001", "甲");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput("S002", "乙"));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    // ---------- 編輯 ----------

    [Fact]
    public async Task Edit_UpdatesFieldsAndKeepsActiveStateAndTransactionFields()
    {
        int id;
        using (var seed = _testDb.CreateContext())
        {
            var entity = new t_4060Supplier
            {
                No = "S001",
                Name = "舊名稱",
                IsActive = true,
                CreditBalance = 500m,
                LastTransDate = new DateTime(2026, 1, 1),
                TemporaryAmount = 7m
            };
            seed.t_4060Supplier.Add(entity);
            seed.SaveChanges();
            entity.IsActive = false;
            seed.SaveChanges();
            id = entity.Id;
        }
        using var db = _testDb.CreateContext();
        var input = ValidInput("S001", "新名稱");
        input.Id = id;
        input.PayDays = 45;

        var result = await CreateService(db).Edit(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadSupplier(id);
        Assert.Equal("新名稱", saved.Name);
        Assert.Equal(45, saved.PayDays);
        Assert.False(saved.IsActive);
        Assert.Equal(500m, saved.CreditBalance);
        Assert.Equal(new DateTime(2026, 1, 1), saved.LastTransDate);
        Assert.Equal(7m, saved.TemporaryAmount);
    }

    [Fact]
    public async Task Edit_KeepingOwnNoAndTaxIdNumber_IsAllowed()
    {
        var id = SeedSupplier("S001", "甲", taxIdNumber: "12345678");
        using var db = _testDb.CreateContext();
        var input = ValidInput("S001", "甲改名");
        input.Id = id;
        input.TaxIdNumber = "12345678";

        var result = await CreateService(db).Edit(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Edit_NoUsedByAnotherSupplier_IsRejected()
    {
        SeedSupplier("S001", "甲");
        var id = SeedSupplier("S002", "乙");
        using var db = _testDb.CreateContext();
        var input = ValidInput("S001", "乙");
        input.Id = id;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Equal("S002", LoadSupplier(id).No);
    }

    [Fact]
    public async Task Edit_TaxIdNumberUsedByAnotherSupplier_IsRejected()
    {
        SeedSupplier("S001", "甲", taxIdNumber: "12345678");
        var id = SeedSupplier("S002", "乙");
        using var db = _testDb.CreateContext();
        var input = ValidInput("S002", "乙");
        input.Id = id;
        input.TaxIdNumber = "12345678";

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Null(LoadSupplier(id).TaxIdNumber);
    }

    [Fact]
    public async Task Edit_ReferencedSupplier_CanStillChangeNoAndTaxIdNumber()
    {
        var id = SeedSupplier("S001", "甲");
        SeedPurchase(id);
        SeedInventory(id);
        using var db = _testDb.CreateContext();
        var input = ValidInput("S001-更正", "甲");
        input.Id = id;
        input.TaxIdNumber = "87654321";

        var result = await CreateService(db).Edit(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadSupplier(id);
        Assert.Equal("S001-更正", saved.No);
        Assert.Equal("87654321", saved.TaxIdNumber);
    }

    [Fact]
    public async Task Edit_LegacySupplierWithoutNo_RequiresNoToBeFilled()
    {
        var id = SeedSupplier(null, "舊資料");
        using var db = _testDb.CreateContext();
        var input = ValidInput("S001", "舊資料");
        input.Id = id;
        input.No = null;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_NotFound_ReturnsNotFoundData()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.Id = 999;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 停用與啟用 ----------

    [Fact]
    public async Task Disable_ActiveSupplier_BecomesInactive()
    {
        var id = SeedSupplier("S001", "甲");
        SeedSupplier("S002", "乙");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(LoadSupplier(id).IsActive);
    }

    [Fact]
    public async Task Disable_LastActiveSupplier_IsAllowed()
    {
        var id = SeedSupplier("S001", "甲");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(LoadSupplier(id).IsActive);
    }

    [Fact]
    public async Task Disable_ReferencedSupplier_IsAllowed()
    {
        var id = SeedSupplier("S001", "甲");
        SeedPurchase(id);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(LoadSupplier(id).IsActive);
    }

    [Fact]
    public async Task Disable_AlreadyInactive_Succeeds()
    {
        var id = SeedSupplier("S001", "甲", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(LoadSupplier(id).IsActive);
    }

    [Fact]
    public async Task Disable_NotFound_ReturnsNotFoundData()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Enable_InactiveSupplier_BecomesActive()
    {
        var id = SeedSupplier("S001", "甲", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Enable(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.True(LoadSupplier(id).IsActive);
    }

    [Fact]
    public async Task Enable_NotFound_ReturnsNotFoundData()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Enable(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 刪除 ----------

    [Fact]
    public async Task Delete_UnreferencedSupplier_IsRemoved()
    {
        var id = SeedSupplier("S001", "甲");
        SeedSupplier("S002", "乙");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(db.t_4060Supplier.Any(x => x.Id == id));
    }

    [Fact]
    public async Task Delete_LastActiveSupplier_IsAllowedWhenUnreferenced()
    {
        var id = SeedSupplier("S001", "甲");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Empty(db.t_4060Supplier);
    }

    [Fact]
    public async Task Delete_SupplierReferencedByPurchase_IsRejectedWithHintToDisable()
    {
        var id = SeedSupplier("S001", "甲");
        SeedPurchase(id);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.Equal(ErrorCodeType.InvalidRequest, result.ErrorCode);
        Assert.Contains("停用", result.ErrorMessage);
        Assert.True(db.t_4060Supplier.Any(x => x.Id == id));
    }

    [Fact]
    public async Task Delete_SupplierReferencedByInventory_IsRejectedWithHintToDisable()
    {
        var id = SeedSupplier("S001", "甲");
        SeedInventory(id);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.Equal(ErrorCodeType.InvalidRequest, result.ErrorCode);
        Assert.Contains("停用", result.ErrorMessage);
        Assert.True(db.t_4060Supplier.Any(x => x.Id == id));
    }

    [Fact]
    public async Task Delete_SupplierWithPrices_RemovesPricesTogether()
    {
        var id = SeedSupplier("S001", "甲");
        var otherId = SeedSupplier("S002", "乙");
        SeedPrice(id, "P001");
        SeedPrice(id, "P002");
        SeedPrice(otherId, "P001");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var check = _testDb.CreateContext();
        Assert.False(check.t_4060Supplier.Any(x => x.Id == id));
        Assert.False(check.t_4061SupplierPrice.Any(x => x.SupplierId == id));
        Assert.Equal(1, check.t_4061SupplierPrice.Count(x => x.SupplierId == otherId));
    }

    [Fact]
    public async Task Delete_ReferencedSupplierWithPrices_IsRejectedAndKeepsPrices()
    {
        var purchaseSupplier = SeedSupplier("S001", "甲");
        var inventorySupplier = SeedSupplier("S002", "乙");
        SeedPurchase(purchaseSupplier);
        SeedInventory(inventorySupplier);
        SeedPrice(purchaseSupplier, "P001");
        SeedPrice(inventorySupplier, "P001");
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var a = await service.Delete(purchaseSupplier);
        var b = await service.Delete(inventorySupplier);

        Assert.Equal(ErrorCodeType.InvalidRequest, a.ErrorCode);
        Assert.Equal(ErrorCodeType.InvalidRequest, b.ErrorCode);
        using var check = _testDb.CreateContext();
        Assert.Equal(2, check.t_4061SupplierPrice.Count());
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFoundData()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 列表與詳情 ----------

    [Fact]
    public async Task Index_ListsActiveFirstThenByNo()
    {
        SeedSupplier("S003", "丙", isActive: false);
        SeedSupplier("S002", "乙");
        SeedSupplier("S001", "甲");
        SeedSupplier("S000", "丁", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierSearchVM());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { "S001", "S002", "S000", "S003" }, result.Data.Items.Select(x => x.No));
    }

    [Theory]
    [InlineData("S00", 3)]
    [InlineData("太平", 1)]
    [InlineData("1234", 1)]
    [InlineData("不存在", 0)]
    public async Task Index_KeywordMatchesNoNameAndTaxIdNumber(string keyword, int expectedCount)
    {
        SeedSupplier("S001", "太平洋供應商", taxIdNumber: "12345678");
        SeedSupplier("S002", "大西洋供應商");
        SeedSupplier("S003", "印度洋供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new SupplierSearchVM { Keyword = keyword });

        Assert.Equal(expectedCount, result.Data.Items.Count());
    }

    [Fact]
    public async Task Index_FiltersByActiveState()
    {
        SeedSupplier("S001", "甲");
        SeedSupplier("S002", "乙", isActive: false);
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var active = await service.Index(new SupplierSearchVM { IsActive = true });
        var inactive = await service.Index(new SupplierSearchVM { IsActive = false });

        Assert.Equal(new[] { "S001" }, active.Data.Items.Select(x => x.No));
        Assert.Equal(new[] { "S002" }, inactive.Data.Items.Select(x => x.No));
    }

    [Fact]
    public async Task Get_ReturnsSupplierDetail()
    {
        int id;
        using (var seed = _testDb.CreateContext())
        {
            var entity = new t_4060Supplier
            {
                No = "S001",
                Name = "甲",
                TaxIdNumber = "12345678",
                Owner = "王大明",
                PayDays = 30,
                CreditLine = 1000m,
                IsActive = true
            };
            seed.t_4060Supplier.Add(entity);
            seed.SaveChanges();
            id = entity.Id;
        }
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Get(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("S001", result.Data.No);
        Assert.Equal("甲", result.Data.Name);
        Assert.Equal("12345678", result.Data.TaxIdNumber);
        Assert.Equal("王大明", result.Data.Owner);
        Assert.Equal(30, result.Data.PayDays);
        Assert.Equal(1000m, result.Data.CreditLine);
        Assert.True(result.Data.IsActive);
    }

    [Fact]
    public async Task Get_NotFound_ReturnsNotFoundData()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Get(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 填單用選單 ----------

    [Fact]
    public async Task GetSelect_ListsOnlyActiveSuppliersWithNoAndName()
    {
        SeedSupplier("S002", "乙供應商");
        SeedSupplier("S001", "甲供應商");
        SeedSupplier("S003", "已停用供應商", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(includeId: null);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { "S001 甲供應商", "S002 乙供應商" }, result.Data.Items.Select(x => x.Text));
        Assert.All(result.Data.Items, x => Assert.True(x.IsActive));
    }

    [Fact]
    public async Task GetSelect_IncludeId_ReturnsDisabledSupplierMarkedInactive()
    {
        SeedSupplier("S001", "甲供應商");
        var disabledId = SeedSupplier("S002", "已停用供應商", isActive: false);
        SeedSupplier("S003", "另一間已停用", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(disabledId);

        Assert.Equal(2, result.Data.Items.Count());
        var disabled = Assert.Single(result.Data.Items, x => x.Value == disabledId);
        Assert.False(disabled.IsActive);
        Assert.Equal("S002 已停用供應商", disabled.Text);
    }

    [Fact]
    public async Task GetSelect_IncludeIdOfActiveSupplier_DoesNotDuplicate()
    {
        var id = SeedSupplier("S001", "甲供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(id);

        Assert.Single(result.Data.Items);
    }

    [Fact]
    public async Task GetSelect_SupplierWithoutNo_ShowsNameOnly()
    {
        SeedSupplier(null, "舊資料供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(null);

        Assert.Equal("舊資料供應商", Assert.Single(result.Data.Items).Text);
    }

    [Fact]
    public async Task GetSelect_SameNameSuppliers_AreToldApartByNo()
    {
        SeedSupplier("S001", "同名供應商");
        SeedSupplier("S002", "同名供應商");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(null);

        Assert.Equal(new[] { "S001 同名供應商", "S002 同名供應商" }, result.Data.Items.Select(x => x.Text));
    }

    [Fact]
    public async Task GetSupplierSelect_LegacySharedEndpoint_ListsOnlyActiveSuppliers()
    {
        SeedSupplier("S001", "啟用中");
        SeedSupplier("S002", "已停用", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSupplierSelect();

        Assert.Equal("啟用中", Assert.Single(result.Data.Items).Text);
    }
}
