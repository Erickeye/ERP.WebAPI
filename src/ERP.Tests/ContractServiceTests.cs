using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._3000Contract;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._3000Contract;
using ERP.Service.API._3000Contract;
using ERP.Tests.Support;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 合約單服務的行為測試（草稿階段）：建立、編輯、刪除、類型與對象、欄位驗證、合約編號唯一、停用對象、檢視與列表。
/// </summary>
public class ContractServiceTests : IDisposable
{
    private const int CurrentUserId = 7;
    private const int OtherUserId = 8;

    private readonly TestDb _testDb = new();
    // 同一個測試內所有服務共用同一個流水號產生器，避免兩張單據拿到相同單號
    private readonly FakeSerialService _serial = new();
    private readonly int _supplierId;
    private readonly int _customerId;
    private readonly int _companyId;

    public ContractServiceTests()
    {
        using var db = _testDb.CreateContext();
        db.User.Add(NewUser(CurrentUserId, "me"));
        db.User.Add(NewUser(OtherUserId, "other"));
        var customer = new t_2000Customer
        {
            TaxInvoiceNumber = "12345678",
            Name = "測試客戶",
            Owner = "負責人",
            RegisteredAddress = "登記地址"
        };
        db.t_2000Customer.Add(customer);
        db.SaveChanges();
        var company = SeedData.CreateCompany(db, "預設公司", "00000000", isActive: true);
        var supplier = SeedData.CreateSupplier(db, "S001", "預設供應商");

        _companyId = company.Id;
        _supplierId = supplier.Id;
        _customerId = customer.Id;
    }

    public void Dispose() => _testDb.Dispose();

    // ---------- 測試輔助 ----------

    private static User NewUser(int id, string account) => new User
    {
        Id = id,
        RoleId = 1,
        CreateDate = new DateTime(2026, 1, 1),
        Name = account,
        Account = account,
        Pwd = "x"
    };

    private I_3050ContractService CreateService(int userId = CurrentUserId)
    {
        var db = _testDb.CreateContext();
        return new _3050ContractService(db, new FakeCurrentUser { UserId = userId }, _serial);
    }

    private ContractAddVM NewSupplierContract(string contractNo = "SC-001") => new ContractAddVM
    {
        Type = ContractType.供應商合約,
        SupplierId = _supplierId,
        CompanyId = _companyId,
        ContractNo = contractNo,
        StartDate = new DateTime(2026, 1, 1),
        EndDate = new DateTime(2026, 12, 31),
        Amount = 1200000m,
        Remark = "年度框架合約"
    };

    private ContractAddVM NewCustomerContract(string contractNo = "CC-001") => new ContractAddVM
    {
        Type = ContractType.客戶合約,
        CustomerId = _customerId,
        CompanyId = _companyId,
        ContractNo = contractNo,
        StartDate = new DateTime(2026, 1, 1),
        EndDate = new DateTime(2026, 12, 31),
        Amount = 500000m
    };

    private int AddCompany(string attribName, string taxInvoiceNumber, bool isActive = true)
    {
        using var db = _testDb.CreateContext();
        return SeedData.CreateCompany(db, attribName, taxInvoiceNumber, isActive).Id;
    }

    private int AddSupplier(string no, string name, bool isActive = true)
    {
        using var db = _testDb.CreateContext();
        return SeedData.CreateSupplier(db, no, name, isActive).Id;
    }

    private void SetCompanyActive(int companyId, bool isActive)
    {
        using var db = _testDb.CreateContext();
        db.t_1080Company.Single(x => x.Id == companyId).IsActive = isActive;
        db.SaveChanges();
    }

    private void SetSupplierActive(int supplierId, bool isActive)
    {
        using var db = _testDb.CreateContext();
        db.t_4060Supplier.Single(x => x.Id == supplierId).IsActive = isActive;
        db.SaveChanges();
    }

    private void SetStatus(int id, ContractStatus status)
    {
        using var db = _testDb.CreateContext();
        db.t_3050Contract.Single(x => x.Id == id).Status = (int)status;
        db.SaveChanges();
    }

    private t_3050Contract Load(int id)
    {
        using var db = _testDb.CreateContext();
        return db.t_3050Contract.Single(x => x.Id == id);
    }

    private async Task<int> AddSupplierContractAsync(string contractNo = "SC-001")
    {
        var result = await CreateService().Add(NewSupplierContract(contractNo));
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return result.Data!.Id;
    }

    private static void AssertInvalid(ResultModel<ContractSaveResultVM> result, string messagePart)
    {
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains(messagePart, result.ErrorMessage);
    }

    // ---------- 建立 ----------

    [Fact]
    public async Task Add_供應商合約建立草稿並帶入單號與建單人()
    {
        var result = await CreateService().Add(NewSupplierContract());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.StartsWith("CT", result.Data!.No);
        var contract = Load(result.Data.Id);
        Assert.Equal(result.Data.No, contract.No);
        Assert.Equal((int)ContractStatus.草稿, contract.Status);
        Assert.Equal((int)ContractType.供應商合約, contract.Type);
        Assert.Equal(_supplierId, contract.SupplierId);
        Assert.Null(contract.CustomerId);
        Assert.Equal(_companyId, contract.CompanyId);
        Assert.Equal("SC-001", contract.ContractNo);
        Assert.Equal(new DateTime(2026, 1, 1), contract.StartDate);
        Assert.Equal(new DateTime(2026, 12, 31), contract.EndDate);
        Assert.Equal(1200000m, contract.Amount);
        Assert.Equal("年度框架合約", contract.Remark);
        Assert.Null(contract.AttachmentPath);
        Assert.Equal(CurrentUserId, contract.CreateUserId);
        Assert.Equal(CurrentUserId, contract.UpdateUserId);
    }

    [Fact]
    public async Task Add_客戶合約建立草稿()
    {
        var result = await CreateService().Add(NewCustomerContract());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var contract = Load(result.Data!.Id);
        Assert.Equal((int)ContractType.客戶合約, contract.Type);
        Assert.Equal(_customerId, contract.CustomerId);
        Assert.Null(contract.SupplierId);
    }

    [Fact]
    public async Task Add_兩張合約單取得不同單號()
    {
        var first = await CreateService().Add(NewSupplierContract("A-1"));
        var second = await CreateService().Add(NewSupplierContract("A-2"));

        Assert.NotEqual(first.Data!.No, second.Data!.No);
    }

    [Fact]
    public async Task Add_備註可以不填()
    {
        var vm = NewSupplierContract();
        vm.Remark = null;

        var result = await CreateService().Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Null(Load(result.Data!.Id).Remark);
    }

    // ---------- 類型與對象 ----------

    [Fact]
    public async Task Add_供應商合約沒選供應商被擋下()
    {
        var vm = NewSupplierContract();
        vm.SupplierId = null;

        AssertInvalid(await CreateService().Add(vm), "供應商");
    }

    [Fact]
    public async Task Add_供應商合約同時填客戶被擋下()
    {
        var vm = NewSupplierContract();
        vm.CustomerId = _customerId;

        AssertInvalid(await CreateService().Add(vm), "客戶");
    }

    [Fact]
    public async Task Add_客戶合約沒選客戶被擋下()
    {
        var vm = NewCustomerContract();
        vm.CustomerId = null;

        AssertInvalid(await CreateService().Add(vm), "客戶");
    }

    [Fact]
    public async Task Add_客戶合約同時填供應商被擋下()
    {
        var vm = NewCustomerContract();
        vm.SupplierId = _supplierId;

        AssertInvalid(await CreateService().Add(vm), "供應商");
    }

    [Fact]
    public async Task Add_合約類型未填被擋下()
    {
        var vm = NewSupplierContract();
        vm.Type = null;

        AssertInvalid(await CreateService().Add(vm), "合約類型");
    }

    [Fact]
    public async Task Add_供應商不存在被擋下()
    {
        var vm = NewSupplierContract();
        vm.SupplierId = 99999;

        AssertInvalid(await CreateService().Add(vm), "找不到供應商");
    }

    [Fact]
    public async Task Add_客戶不存在被擋下()
    {
        var vm = NewCustomerContract();
        vm.CustomerId = 99999;

        AssertInvalid(await CreateService().Add(vm), "找不到客戶");
    }

    // ---------- 欄位驗證 ----------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Add_合約編號必填(string? contractNo)
    {
        var vm = NewSupplierContract();
        vm.ContractNo = contractNo;

        AssertInvalid(await CreateService().Add(vm), "合約編號");
    }

    [Fact]
    public async Task Add_合約編號超過長度被擋下()
    {
        var vm = NewSupplierContract(new string('A', 65));

        AssertInvalid(await CreateService().Add(vm), "合約編號");
    }

    [Fact]
    public async Task Add_合約編號前後空白會被修剪()
    {
        var result = await CreateService().Add(NewSupplierContract("  SC-TRIM  "));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("SC-TRIM", Load(result.Data!.Id).ContractNo);
    }

    [Fact]
    public async Task Add_備註超過長度被擋下()
    {
        var vm = NewSupplierContract();
        vm.Remark = new string('備', 1025);

        AssertInvalid(await CreateService().Add(vm), "備註");
    }

    [Fact]
    public async Task Add_簽約公司未選被擋下()
    {
        var vm = NewSupplierContract();
        vm.CompanyId = null;

        AssertInvalid(await CreateService().Add(vm), "簽約公司");
    }

    [Fact]
    public async Task Add_簽約公司不存在被擋下()
    {
        var vm = NewSupplierContract();
        vm.CompanyId = 99999;

        AssertInvalid(await CreateService().Add(vm), "找不到簽約公司");
    }

    [Fact]
    public async Task Add_起迄日未填被擋下()
    {
        var vm = NewSupplierContract();
        vm.StartDate = null;

        AssertInvalid(await CreateService().Add(vm), "起日與迄日");
    }

    [Fact]
    public async Task Add_迄日早於起日被擋下()
    {
        var vm = NewSupplierContract();
        vm.StartDate = new DateTime(2026, 6, 2);
        vm.EndDate = new DateTime(2026, 6, 1);

        AssertInvalid(await CreateService().Add(vm), "迄日不得早於起日");
    }

    [Fact]
    public async Task Add_起迄同一天可以()
    {
        var vm = NewSupplierContract();
        vm.StartDate = new DateTime(2026, 6, 1);
        vm.EndDate = new DateTime(2026, 6, 1);

        var result = await CreateService().Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Add_起訖日可以是過去的日期以便補登舊合約()
    {
        var vm = NewSupplierContract();
        vm.StartDate = new DateTime(2020, 1, 1);
        vm.EndDate = new DateTime(2020, 12, 31);

        var result = await CreateService().Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Add_合約總金額未填被擋下()
    {
        var vm = NewSupplierContract();
        vm.Amount = null;

        AssertInvalid(await CreateService().Add(vm), "合約總金額");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public async Task Add_合約總金額必須大於零(double amount)
    {
        var vm = NewSupplierContract();
        vm.Amount = (decimal)amount;

        AssertInvalid(await CreateService().Add(vm), "大於 0");
    }

    [Fact]
    public async Task Add_合約總金額超過兩位小數被擋下()
    {
        var vm = NewSupplierContract();
        vm.Amount = 100.001m;

        AssertInvalid(await CreateService().Add(vm), "兩位小數");
    }

    [Fact]
    public async Task Add_合約總金額兩位小數可以()
    {
        var vm = NewSupplierContract();
        vm.Amount = 100.25m;

        var result = await CreateService().Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(100.25m, Load(result.Data!.Id).Amount);
    }

    [Fact]
    public async Task Add_合約總金額超過上限被擋下()
    {
        var vm = NewSupplierContract();
        vm.Amount = 10000000000m;

        AssertInvalid(await CreateService().Add(vm), "上限");
    }

    [Fact]
    public async Task Add_驗證失敗時不留下任何資料()
    {
        var vm = NewSupplierContract();
        vm.Amount = 0;

        await CreateService().Add(vm);

        using var db = _testDb.CreateContext();
        Assert.Empty(db.t_3050Contract);
    }

    // ---------- 合約編號唯一 ----------

    [Fact]
    public async Task Add_合約編號與非作廢單重複被擋下()
    {
        await AddSupplierContractAsync("DUP-1");

        AssertInvalid(await CreateService().Add(NewSupplierContract("DUP-1")), "合約編號已存在");
    }

    [Fact]
    public async Task Add_合約編號跨類型仍不可重複()
    {
        await AddSupplierContractAsync("DUP-2");

        AssertInvalid(await CreateService().Add(NewCustomerContract("DUP-2")), "合約編號已存在");
    }

    [Theory]
    [InlineData(ContractStatus.草稿)]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已退回)]
    public async Task Add_草稿簽核中已核准已退回的單都佔用合約編號(ContractStatus status)
    {
        var id = await AddSupplierContractAsync("DUP-3");
        SetStatus(id, status);

        AssertInvalid(await CreateService().Add(NewSupplierContract("DUP-3")), "合約編號已存在");
    }

    [Fact]
    public async Task Add_作廢單不佔用合約編號可以重開新單()
    {
        var id = await AddSupplierContractAsync("DUP-4");
        SetStatus(id, ContractStatus.已作廢);

        var result = await CreateService().Add(NewSupplierContract("DUP-4"));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Edit_保留自己的合約編號可以儲存()
    {
        var id = await AddSupplierContractAsync("KEEP-1");
        var vm = NewSupplierContract("KEEP-1");
        vm.Id = id;
        vm.Amount = 999m;

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(999m, Load(id).Amount);
    }

    [Fact]
    public async Task Edit_改成別張單的合約編號被擋下()
    {
        await AddSupplierContractAsync("OWN-A");
        var id = await AddSupplierContractAsync("OWN-B");
        var vm = NewSupplierContract("OWN-A");
        vm.Id = id;

        AssertInvalid(await CreateService().Edit(vm), "合約編號已存在");
    }

    [Fact]
    public async Task Edit_可以更正合約編號()
    {
        var id = await AddSupplierContractAsync("TYPO");
        var vm = NewSupplierContract("FIXED");
        vm.Id = id;

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("FIXED", Load(id).ContractNo);
    }

    // ---------- 停用的供應商與簽約公司 ----------

    [Fact]
    public async Task Add_供應商已停用被擋下()
    {
        var disabled = AddSupplier("S002", "停用供應商", isActive: false);
        var vm = NewSupplierContract();
        vm.SupplierId = disabled;

        AssertInvalid(await CreateService().Add(vm), "已停用");
    }

    [Fact]
    public async Task Add_簽約公司已停用被擋下()
    {
        var vm = NewSupplierContract();
        vm.CompanyId = AddCompany("停用公司", "22222222", isActive: false);

        AssertInvalid(await CreateService().Add(vm), "已停用");
    }

    [Fact]
    public async Task Edit_維持原本已停用的供應商仍可儲存其他欄位()
    {
        var id = await AddSupplierContractAsync();
        SetSupplierActive(_supplierId, false);
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.Remark = "停用後仍更正備註";

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("停用後仍更正備註", Load(id).Remark);
    }

    [Fact]
    public async Task Edit_改選已停用的供應商被擋下()
    {
        var id = await AddSupplierContractAsync();
        var disabled = AddSupplier("S002", "停用供應商", isActive: false);
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.SupplierId = disabled;

        AssertInvalid(await CreateService().Edit(vm), "已停用");
    }

    [Fact]
    public async Task Edit_維持原本已停用的簽約公司仍可儲存其他欄位()
    {
        var id = await AddSupplierContractAsync();
        SetCompanyActive(_companyId, false);
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.Amount = 1m;

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(1m, Load(id).Amount);
    }

    [Fact]
    public async Task Edit_改選已停用的簽約公司被擋下()
    {
        var id = await AddSupplierContractAsync();
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.CompanyId = AddCompany("停用公司", "22222222", isActive: false);

        AssertInvalid(await CreateService().Edit(vm), "已停用");
    }

    // ---------- 編輯 ----------

    [Fact]
    public async Task Edit_草稿可以修改並更新最後修改人()
    {
        var id = await AddSupplierContractAsync();
        var vm = NewCustomerContract("CC-NEW");
        vm.Id = id;
        vm.Amount = 888m;
        vm.Remark = "改成客戶合約";

        var result = await CreateService(OtherUserId).Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var contract = Load(id);
        Assert.Equal((int)ContractType.客戶合約, contract.Type);
        Assert.Equal(_customerId, contract.CustomerId);
        Assert.Null(contract.SupplierId);
        Assert.Equal("CC-NEW", contract.ContractNo);
        Assert.Equal(888m, contract.Amount);
        Assert.Equal(CurrentUserId, contract.CreateUserId);
        Assert.Equal(OtherUserId, contract.UpdateUserId);
    }

    [Fact]
    public async Task Edit_已退回可以修改()
    {
        var id = await AddSupplierContractAsync();
        SetStatus(id, ContractStatus.已退回);
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.Amount = 777m;

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(777m, Load(id).Amount);
        Assert.Equal((int)ContractStatus.已退回, Load(id).Status);
    }

    [Theory]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已作廢)]
    public async Task Edit_簽核中已核准已作廢不可編輯且訊息帶目前狀態(ContractStatus status)
    {
        var id = await AddSupplierContractAsync();
        SetStatus(id, status);
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.Amount = 1m;

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
        Assert.Equal(1200000m, Load(id).Amount);
    }

    [Fact]
    public async Task Edit_找不到合約單回傳找不到資料()
    {
        var vm = NewSupplierContract();
        vm.Id = 99999;

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_驗證失敗時資料不被改動()
    {
        var id = await AddSupplierContractAsync();
        var vm = NewSupplierContract();
        vm.Id = id;
        vm.Amount = 0;

        await CreateService().Edit(vm);

        Assert.Equal(1200000m, Load(id).Amount);
    }

    // ---------- 刪除 ----------

    [Fact]
    public async Task Delete_草稿可以刪除()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var db = _testDb.CreateContext();
        Assert.Empty(db.t_3050Contract);
    }

    [Theory]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已退回)]
    [InlineData(ContractStatus.已作廢)]
    public async Task Delete_非草稿不可刪除且訊息帶目前狀態(ContractStatus status)
    {
        var id = await AddSupplierContractAsync();
        SetStatus(id, status);

        var result = await CreateService().Delete(id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
        using var db = _testDb.CreateContext();
        Assert.Single(db.t_3050Contract);
    }

    [Fact]
    public async Task Delete_找不到合約單回傳找不到資料()
    {
        var result = await CreateService().Delete(99999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 檢視 ----------

    [Fact]
    public async Task Get_供應商合約帶出對象簽約公司與顯示欄位()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().Get(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var data = result.Data!;
        Assert.Equal("SC-001", data.ContractNo);
        Assert.Equal((int)ContractType.供應商合約, data.Type);
        Assert.Equal("供應商合約", data.TypeDisplay);
        Assert.Equal("預設供應商", data.PartyName);
        Assert.True(data.SupplierIsActive);
        Assert.Equal("預設公司", data.CompanyName);
        Assert.True(data.CompanyIsActive);
        Assert.Equal(1200000m, data.Amount);
        Assert.False(data.HasAttachment);
        Assert.Equal((int)ContractStatus.草稿, data.Status);
        Assert.Equal("草稿", data.StatusDisplay);
        Assert.Equal("me", data.CreateUserName);
    }

    [Fact]
    public async Task Get_客戶合約對象顯示客戶名稱()
    {
        var added = await CreateService().Add(NewCustomerContract());

        var result = await CreateService().Get(added.Data!.Id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal("客戶合約", result.Data!.TypeDisplay);
        Assert.Equal("測試客戶", result.Data.PartyName);
        Assert.Null(result.Data.SupplierIsActive);
    }

    [Fact]
    public async Task Get_對象供應商停用後仍顯示原供應商並標示停用()
    {
        var id = await AddSupplierContractAsync();
        SetSupplierActive(_supplierId, false);

        var result = await CreateService().Get(id);

        Assert.Equal("預設供應商", result.Data!.PartyName);
        Assert.False(result.Data.SupplierIsActive);
    }

    [Fact]
    public async Task Get_找不到合約單回傳找不到資料()
    {
        var result = await CreateService().Get(99999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 列表 ----------

    [Fact]
    public async Task Index_預設依合約起日由新到舊()
    {
        var older = NewSupplierContract("OLD");
        older.StartDate = new DateTime(2025, 1, 1);
        older.EndDate = new DateTime(2025, 12, 31);
        var newer = NewSupplierContract("NEW");
        newer.StartDate = new DateTime(2026, 6, 1);
        newer.EndDate = new DateTime(2026, 12, 31);
        await CreateService().Add(older);
        await CreateService().Add(newer);

        var result = await CreateService().Index(new ContractSearchVM());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { "NEW", "OLD" }, result.Data!.Items.Select(x => x.ContractNo).ToArray());
        Assert.Equal(2, result.Data.TotalCount);
    }

    [Fact]
    public async Task Index_起日相同時新建立的在前()
    {
        await CreateService().Add(NewSupplierContract("FIRST"));
        await CreateService().Add(NewSupplierContract("SECOND"));

        var result = await CreateService().Index(new ContractSearchVM());

        Assert.Equal(new[] { "SECOND", "FIRST" }, result.Data!.Items.Select(x => x.ContractNo).ToArray());
    }

    [Fact]
    public async Task Index_分頁只回傳該頁並帶總筆數()
    {
        for (var i = 1; i <= 5; i++)
        {
            var vm = NewSupplierContract($"P-{i}");
            vm.StartDate = new DateTime(2026, i, 1);
            vm.EndDate = new DateTime(2026, i, 28);
            await CreateService().Add(vm);
        }

        var result = await CreateService().Index(new ContractSearchVM { PageNumber = 2, PageSize = 2 });

        Assert.Equal(5, result.Data!.TotalCount);
        Assert.Equal(new[] { "P-3", "P-2" }, result.Data.Items.Select(x => x.ContractNo).ToArray());
    }

    [Fact]
    public async Task Index_可依指定欄位排序()
    {
        // 用字串欄位驗證排序：SQLite 不支援 decimal 排序，金額排序須在真實資料庫驗證
        await CreateService().Add(NewSupplierContract("S"));
        await CreateService().Add(NewSupplierContract("B"));

        var result = await CreateService().Index(new ContractSearchVM { SortColumn = "ContractNo", SortDirection = "asc" });

        Assert.Equal(new[] { "B", "S" }, result.Data!.Items.Select(x => x.ContractNo).ToArray());
    }

    [Fact]
    public async Task Index_列表同時含供應商合約與客戶合約並帶顯示欄位()
    {
        await CreateService().Add(NewSupplierContract());
        await CreateService().Add(NewCustomerContract());

        var result = await CreateService().Index(new ContractSearchVM());

        Assert.Equal(2, result.Data!.Items.Count);
        Assert.Contains(result.Data.Items, x => x.TypeDisplay == "供應商合約" && x.PartyName == "預設供應商");
        Assert.Contains(result.Data.Items, x => x.TypeDisplay == "客戶合約" && x.PartyName == "測試客戶");
        Assert.All(result.Data.Items, x => Assert.Equal("草稿", x.StatusDisplay));
    }
}
