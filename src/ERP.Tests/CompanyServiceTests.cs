using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.Tests.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 公司服務的行為測試：新增、編輯、欄位驗證、簡稱與統編唯一、停用與啟用、刪除、填單選單與列表。
/// 被領料單（ticket 02）與出差單（ticket 03）引用過的公司不可刪除，都已涵蓋。
/// </summary>
public class CompanyServiceTests : IDisposable
{
    private readonly TestDb _testDb = new();

    public void Dispose() => _testDb.Dispose();

    // ---------- 測試輔助 ----------

    private _1080CompanyService CreateService(TestERPDbContext db) => new(db);

    private static CompanyAddVM ValidInput(
        string attribName = "甲公司",
        string taxInvoiceNumber = "12345678") => new()
    {
        AttribName = attribName,
        Name = attribName + "股份有限公司",
        TaxInvoiceNumber = taxInvoiceNumber,
        TaxSerialNumber = "123456789",
        Owner = "王大明",
        RegisteredAddress = "台北市測試路 1 號"
    };

    /// <summary>
    /// 直接寫入一間公司。IsActive 的資料表預設值是 true，EF 會把 false（CLR 預設值）視為「未指定」而改用預設值，
    /// 所以要建立停用的公司時，先存成啟用再改成停用。
    /// </summary>
    private int SeedCompany(string attribName, string taxInvoiceNumber, bool isActive = true)
    {
        using var db = _testDb.CreateContext();
        var entity = new t_1080Company
        {
            AttribName = attribName,
            Name = attribName + "股份有限公司",
            TaxInvoiceNumber = taxInvoiceNumber,
            TaxSerialNumber = "123456789",
            Owner = "王大明",
            RegisteredAddress = "台北市測試路 1 號",
            IsActive = true
        };
        db.t_1080Company.Add(entity);
        db.SaveChanges();

        if (!isActive)
        {
            entity.IsActive = false;
            db.SaveChanges();
        }
        return entity.Id;
    }

    private t_1080Company LoadCompany(int id)
    {
        using var db = _testDb.CreateContext();
        return db.t_1080Company.AsNoTracking().Single(x => x.Id == id);
    }

    // ---------- 新增 ----------

    [Fact]
    public async Task Add_ValidInput_SavesActiveCompanyWithTrimmedValues()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.AttribName = "  甲公司  ";
        input.Owner = " 王大明 ";

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadCompany(result.Data);
        Assert.Equal("甲公司", saved.AttribName);
        Assert.Equal("王大明", saved.Owner);
        Assert.Equal("12345678", saved.TaxInvoiceNumber);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Add_WithOptionalFields_SavesThem()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.ContactPhone = "02-12345678";
        input.FaxPhone = "02-87654321";
        input.DeliveryAddress = "新北市送貨路 2 號";
        input.TaxInvoiceAddress = "新北市發票路 3 號";
        input.BankName = "測試銀行";
        input.CheckingAccount = "111";
        input.RemittanceAccount = "222";
        input.PayDays = "30";
        input.FoundedDate = new DateTime(2010, 5, 20);
        input.InvoiceForm = "三聯式";

        var result = await CreateService(db).Add(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadCompany(result.Data);
        Assert.Equal("02-12345678", saved.ContactPhone);
        Assert.Equal("02-87654321", saved.FaxPhone);
        Assert.Equal("新北市送貨路 2 號", saved.DeliveryAddress);
        Assert.Equal("新北市發票路 3 號", saved.TaxInvoiceAddress);
        Assert.Equal("測試銀行", saved.BankName);
        Assert.Equal("111", saved.CheckingAccount);
        Assert.Equal("222", saved.RemittanceAccount);
        Assert.Equal("30", saved.PayDays);
        Assert.Equal(new DateTime(2010, 5, 20), saved.FoundedDate);
        Assert.Equal("三聯式", saved.InvoiceForm);
    }

    [Theory]
    [InlineData("AttribName")]
    [InlineData("Name")]
    [InlineData("TaxInvoiceNumber")]
    [InlineData("TaxSerialNumber")]
    [InlineData("Owner")]
    [InlineData("RegisteredAddress")]
    public async Task Add_MissingRequiredField_IsRejected(string field)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        typeof(CompanyAddVM).GetProperty(field)!.SetValue(input, "   ");

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Empty(db.t_1080Company);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567A")]
    [InlineData("１２３４５６７８")] // 全形數字:\d 會放行,必須用 [0-9]
    public async Task Add_TaxInvoiceNumberNotEightDigits_IsRejected(string taxInvoiceNumber)
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(taxInvoiceNumber: taxInvoiceNumber));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("8 碼數字", result.ErrorMessage);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    public async Task Add_TaxSerialNumberNotNineCharacters_IsRejected(string taxSerialNumber)
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.TaxSerialNumber = taxSerialNumber;

        var result = await CreateService(db).Add(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("9 碼", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_FieldLongerThanColumn_IsRejected()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(attribName: new string('字', 33)));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("32", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_DuplicateTaxInvoiceNumber_IsRejectedWithClearMessage()
    {
        SeedCompany("甲公司", "12345678");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(attribName: "乙公司", taxInvoiceNumber: "12345678"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("統一編號", result.ErrorMessage);
        Assert.Equal(1, db.t_1080Company.Count());
    }

    [Fact]
    public async Task Add_DuplicateAttribName_IsRejectedWithClearMessage()
    {
        SeedCompany("甲公司", "12345678");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(attribName: "甲公司", taxInvoiceNumber: "87654321"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("公司簡稱", result.ErrorMessage);
        Assert.Equal(1, db.t_1080Company.Count());
    }

    [Fact]
    public async Task Add_DuplicateEvenWhenTheExistingCompanyIsDisabled_IsRejected()
    {
        // 停用的公司仍佔用簡稱與統編，否則重新啟用時會撞到唯一索引
        SeedCompany("甲公司", "12345678", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Add(ValidInput(attribName: "乙公司", taxInvoiceNumber: "12345678"));

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    // ---------- 取得單筆 ----------

    [Fact]
    public async Task Get_ReturnsTheCompanyWithAllFields()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.BankName = "測試銀行";
        var id = (await CreateService(db).Add(input)).Data;

        var result = await CreateService(db).Get(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("甲公司", result.Data!.AttribName);
        Assert.Equal("甲公司股份有限公司", result.Data.Name);
        Assert.Equal("12345678", result.Data.TaxInvoiceNumber);
        Assert.Equal("測試銀行", result.Data.BankName);
        Assert.True(result.Data.IsActive);
    }

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Get(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 編輯 ----------

    [Fact]
    public async Task Edit_ChangesFields_AndKeepsTheActiveStatus()
    {
        var id = SeedCompany("甲公司", "12345678", isActive: false);
        using var db = _testDb.CreateContext();
        var input = ValidInput(attribName: "甲公司新名", taxInvoiceNumber: "11111111");
        input.Id = id;
        input.Owner = "李小華";

        var result = await CreateService(db).Edit(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var saved = LoadCompany(id);
        Assert.Equal("甲公司新名", saved.AttribName);
        Assert.Equal("11111111", saved.TaxInvoiceNumber);
        Assert.Equal("李小華", saved.Owner);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task Edit_UnknownId_ReturnsNotFound()
    {
        using var db = _testDb.CreateContext();
        var input = ValidInput();
        input.Id = 999;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_KeepingItsOwnAttribNameAndTaxInvoiceNumber_IsNotADuplicate()
    {
        var id = SeedCompany("甲公司", "12345678");
        using var db = _testDb.CreateContext();
        var input = ValidInput(attribName: "甲公司", taxInvoiceNumber: "12345678");
        input.Id = id;
        input.Owner = "李小華";

        var result = await CreateService(db).Edit(input);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("李小華", LoadCompany(id).Owner);
    }

    [Fact]
    public async Task Edit_TaxInvoiceNumberOfAnotherCompany_IsRejected()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321");
        using var db = _testDb.CreateContext();
        var input = ValidInput(attribName: "甲公司", taxInvoiceNumber: "87654321");
        input.Id = id;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("統一編號", result.ErrorMessage);
        Assert.Equal("12345678", LoadCompany(id).TaxInvoiceNumber);
    }

    [Fact]
    public async Task Edit_AttribNameOfAnotherCompany_IsRejected()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321");
        using var db = _testDb.CreateContext();
        var input = ValidInput(attribName: "乙公司", taxInvoiceNumber: "12345678");
        input.Id = id;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("公司簡稱", result.ErrorMessage);
        Assert.Equal("甲公司", LoadCompany(id).AttribName);
    }

    [Fact]
    public async Task Edit_InvalidInput_IsRejectedAndNothingIsSaved()
    {
        var id = SeedCompany("甲公司", "12345678");
        using var db = _testDb.CreateContext();
        var input = ValidInput(taxInvoiceNumber: "123");
        input.Id = id;

        var result = await CreateService(db).Edit(input);

        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Equal("12345678", LoadCompany(id).TaxInvoiceNumber);
    }

    // ---------- 停用與啟用 ----------

    [Fact]
    public async Task Disable_ThenEnable_TogglesTheActiveStatus()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321");
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var disable = await service.Disable(id);
        Assert.True(disable.IsSuccess, disable.ErrorMessage);
        Assert.False(LoadCompany(id).IsActive);

        var enable = await service.Enable(id);
        Assert.True(enable.IsSuccess, enable.ErrorMessage);
        Assert.True(LoadCompany(id).IsActive);
    }

    [Fact]
    public async Task Disable_TheLastActiveCompany_IsRejected()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(id);

        Assert.Equal(ErrorCodeType.InvalidRequest, result.ErrorCode);
        Assert.Contains("至少", result.ErrorMessage);
        Assert.True(LoadCompany(id).IsActive);
    }

    [Fact]
    public async Task Disable_AnAlreadyDisabledCompany_Succeeds()
    {
        SeedCompany("甲公司", "12345678");
        var disabledId = SeedCompany("乙公司", "87654321", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Disable(disabledId);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(LoadCompany(disabledId).IsActive);
    }

    [Fact]
    public async Task DisableAndEnable_UnknownId_ReturnNotFound()
    {
        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        Assert.Equal(ErrorCodeType.NotFoundData, (await service.Disable(999)).ErrorCode);
        Assert.Equal(ErrorCodeType.NotFoundData, (await service.Enable(999)).ErrorCode);
    }

    // ---------- 刪除 ----------

    [Fact]
    public async Task Delete_ADisabledCompanyNobodyUses_IsRemoved()
    {
        SeedCompany("甲公司", "12345678");
        var id = SeedCompany("乙公司", "87654321", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(1, db.t_1080Company.Count());
    }

    [Fact]
    public async Task Delete_AnActiveCompanyWhenAnotherIsActive_IsRemoved()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(1, db.t_1080Company.Count());
    }

    [Fact]
    public async Task Delete_TheLastActiveCompany_IsRejected()
    {
        var id = SeedCompany("甲公司", "12345678");
        SeedCompany("乙公司", "87654321", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(id);

        Assert.Equal(ErrorCodeType.InvalidRequest, result.ErrorCode);
        Assert.Contains("至少", result.ErrorMessage);
        Assert.Equal(2, db.t_1080Company.Count());
    }

    [Fact]
    public async Task Delete_ACompanyUsedByAMaterialIssue_IsRejectedButCanBeDisabled()
    {
        var used = SeedCompany("甲公司", "12345678");
        var other = SeedCompany("乙公司", "87654321");
        using (var seed = _testDb.CreateContext())
        {
            seed.t_4030MaterialIssue.Add(new t_4030MaterialIssue
            {
                No = "IS11510001",
                IssueDate = new DateTime(2026, 10, 1),
                PurposeId = 1,
                LocationId = 1,
                ReceiverStaffId = 1,
                CompanyId = used,
                Status = 0,
                CreateTime = DateTime.Now,
                CreateUserId = 1
            });
            seed.SaveChanges();
        }

        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var deleted = await service.Delete(used);
        Assert.False(deleted.IsSuccess);
        Assert.Contains("停用", deleted.ErrorMessage);
        Assert.NotNull(LoadCompany(used));

        // 被引用的公司仍可停用（還有另一間啟用中）
        var disabled = await service.Disable(used);
        Assert.True(disabled.IsSuccess, disabled.ErrorMessage);
        Assert.False(LoadCompany(used).IsActive);
        Assert.True(LoadCompany(other).IsActive);
    }

    [Fact]
    public async Task Delete_ACompanyUsedByABusinessTrip_IsRejectedButCanBeDisabled()
    {
        var used = SeedCompany("甲公司", "12345678");
        var other = SeedCompany("乙公司", "87654321");
        using (var seed = _testDb.CreateContext())
        {
            seed.t_1060BusinessTrip.Add(new t_1060BusinessTrip
            {
                No = "BT11510001",
                TravelerStaffId = 1,
                CreateUserId = 1,
                Destination = "台中客戶端",
                Reason = "系統上線支援",
                StartDate = new DateTime(2026, 10, 5),
                EndDate = new DateTime(2026, 10, 6),
                Status = 0,
                CreateTime = DateTime.Now,
                CompanyId = used
            });
            seed.SaveChanges();
        }

        using var db = _testDb.CreateContext();
        var service = CreateService(db);

        var deleted = await service.Delete(used);
        Assert.False(deleted.IsSuccess);
        Assert.Contains("停用", deleted.ErrorMessage);
        Assert.NotNull(LoadCompany(used));

        // 被引用的公司仍可停用（還有另一間啟用中）
        var disabled = await service.Disable(used);
        Assert.True(disabled.IsSuccess, disabled.ErrorMessage);
        Assert.False(LoadCompany(used).IsActive);
        Assert.True(LoadCompany(other).IsActive);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Delete(999);

        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 填單用選單 ----------

    [Fact]
    public async Task GetSelect_ReturnsOnlyActiveCompaniesShownByAttribName()
    {
        var firstId = SeedCompany("甲公司", "11111111");
        SeedCompany("丙公司", "33333333", isActive: false);
        var secondId = SeedCompany("乙公司", "22222222");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(includeId: null);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var items = result.Data!.Items;
        Assert.Equal(new[] { firstId, secondId }, items.Select(x => x.Value).ToArray());
        Assert.Equal(new[] { "甲公司", "乙公司" }, items.Select(x => x.Text).ToArray());
        Assert.All(items, x => Assert.True(x.IsActive));
    }

    [Fact]
    public async Task GetSelect_WithIncludeIdOfADisabledCompany_AddsItMarkedAsInactive()
    {
        var activeId = SeedCompany("甲公司", "11111111");
        var disabledId = SeedCompany("丙公司", "33333333", isActive: false);
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(includeId: disabledId);

        var items = result.Data!.Items;
        Assert.Equal(2, items.Count);
        Assert.True(items.Single(x => x.Value == activeId).IsActive);
        var included = items.Single(x => x.Value == disabledId);
        Assert.False(included.IsActive);
        Assert.Equal("丙公司", included.Text);
    }

    [Fact]
    public async Task GetSelect_WithIncludeIdOfAnActiveCompany_DoesNotDuplicateIt()
    {
        var activeId = SeedCompany("甲公司", "11111111");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(includeId: activeId);

        Assert.Single(result.Data!.Items);
    }

    [Fact]
    public async Task GetSelect_WithUnknownIncludeId_IgnoresIt()
    {
        SeedCompany("甲公司", "11111111");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).GetSelect(includeId: 999);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Single(result.Data!.Items);
    }

    // ---------- 列表 ----------

    [Fact]
    public async Task Index_ListsActiveCompaniesFirst()
    {
        var disabledId = SeedCompany("甲公司", "11111111", isActive: false);
        var firstActiveId = SeedCompany("乙公司", "22222222");
        var secondActiveId = SeedCompany("丙公司", "33333333");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new CompanySearchVM());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { firstActiveId, secondActiveId, disabledId }, result.Data!.Items.Select(x => x.Id).ToArray());
        Assert.Equal(3, result.Data.TotalCount);
    }

    [Theory]
    [InlineData("乙公司", "乙公司")]
    [InlineData("甲公司股份", "甲公司")]
    [InlineData("2222", "乙公司")]
    public async Task Index_KeywordMatchesAttribNameFullNameOrTaxInvoiceNumber(string keyword, string expectedAttribName)
    {
        SeedCompany("甲公司", "11111111");
        SeedCompany("乙公司", "22222222");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new CompanySearchVM { Keyword = keyword });

        Assert.Equal(new[] { expectedAttribName }, result.Data!.Items.Select(x => x.AttribName).ToArray());
    }

    [Theory]
    [InlineData(true, "乙公司")]
    [InlineData(false, "甲公司")]
    public async Task Index_FiltersByActiveStatus(bool isActive, string expectedAttribName)
    {
        SeedCompany("甲公司", "11111111", isActive: false);
        SeedCompany("乙公司", "22222222");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new CompanySearchVM { IsActive = isActive });

        Assert.Equal(new[] { expectedAttribName }, result.Data!.Items.Select(x => x.AttribName).ToArray());
    }

    [Fact]
    public async Task Index_PagesTheResultAndReportsTheTotal()
    {
        SeedCompany("甲公司", "11111111");
        SeedCompany("乙公司", "22222222");
        SeedCompany("丙公司", "33333333");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new CompanySearchVM { PageNumber = 2, PageSize = 2 });

        Assert.Single(result.Data!.Items);
        Assert.Equal(3, result.Data.TotalCount);
    }

    [Fact]
    public async Task Index_SortsByTheRequestedColumn()
    {
        SeedCompany("甲公司", "11111111");
        SeedCompany("乙公司", "33333333");
        SeedCompany("丙公司", "22222222");
        using var db = _testDb.CreateContext();

        var result = await CreateService(db).Index(new CompanySearchVM
        {
            SortColumn = nameof(CompanyVM.TaxInvoiceNumber),
            SortDirection = "desc"
        });

        Assert.Equal(new[] { "33333333", "22222222", "11111111" }, result.Data!.Items.Select(x => x.TaxInvoiceNumber).ToArray());
    }
}
