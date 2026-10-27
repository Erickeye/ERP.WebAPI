using ERP.Approval.Services;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._3000Contract;
using ERP.Library.Enums.Login;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._3000Contract;
using ERP.Service.API._3000Contract;
using ERP.Tests.Support;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
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
    private readonly FakeSftpService _sftp = new();
    private readonly FakeApprovalWorkflowService _approval = new();
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

    private I_3050ContractService CreateService(int userId = CurrentUserId, int roleId = 1)
    {
        return CreateService(out _, userId, roleId);
    }

    // 需要讓假引擎在同一個 DbContext 內寫入紀錄（驗證失敗時整筆回滾）時，用這個多載取得服務使用的 DbContext
    private I_3050ContractService CreateService(out TestERPDbContext db, int userId = CurrentUserId, int roleId = 1)
    {
        db = _testDb.CreateContext();
        return new _3050ContractService(db, new FakeCurrentUser { UserId = userId, RoleId = roleId }, _serial, _sftp,
            NullLogger<_3050ContractService>.Instance, _approval);
    }

    private void GrantPermission(int roleId, PermissionType permission)
    {
        using var db = _testDb.CreateContext();
        if (!db.Role.Any(x => x.Id == roleId))
        {
            db.Role.Add(new Role { Id = roleId, RoleName = $"角色{roleId}" });
        }
        db.Permission.Add(new Permission { RoleId = roleId, PageId = (int)permission });
        db.SaveChanges();
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

    // ---------- 合約掃描檔 ----------

    private static readonly byte[] PdfBytes = "%PDF-1.7\n合約內容\n%%EOF"u8.ToArray();

    private static IFormFile NewFile(
        byte[]? content = null, string fileName = "contract.pdf", string contentType = "application/pdf")
    {
        content ??= PdfBytes;
        return new FormFile(new MemoryStream(content), 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static ContractAttachmentVM NewAttachment(int id, IFormFile? file = null) =>
        new ContractAttachmentVM { Id = id, File = file ?? NewFile() };

    [Fact]
    public async Task UploadAttachment_草稿可以上傳PDF並只存遠端路徑()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().UploadAttachment(NewAttachment(id));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var contract = Load(id);
        Assert.False(string.IsNullOrEmpty(contract.AttachmentPath));
        Assert.StartsWith($"/contract-{contract.No}-", contract.AttachmentPath);
        Assert.EndsWith(".pdf", contract.AttachmentPath);
        Assert.Equal(PdfBytes, _sftp.Files[contract.AttachmentPath!]);
        Assert.True((await CreateService().Get(id)).Data!.HasAttachment);
    }

    [Fact]
    public async Task UploadAttachment_遠端路徑不使用上傳者提供的檔名()
    {
        var id = await AddSupplierContractAsync();

        await CreateService().UploadAttachment(NewAttachment(id, NewFile(fileName: "../../etc/passwd.pdf")));

        var path = Load(id).AttachmentPath!;
        Assert.DoesNotContain("..", path);
        Assert.DoesNotContain("passwd", path);
    }

    [Fact]
    public async Task UploadAttachment_已退回可以更換且舊檔被清掉()
    {
        var id = await AddSupplierContractAsync();
        await CreateService().UploadAttachment(NewAttachment(id));
        var oldPath = Load(id).AttachmentPath!;
        SetStatus(id, ContractStatus.已退回);
        var newContent = "%PDF-1.4\n新版"u8.ToArray();

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile(newContent)));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var newPath = Load(id).AttachmentPath!;
        Assert.NotEqual(oldPath, newPath);
        Assert.Equal(newContent, _sftp.Files[newPath]);
        Assert.DoesNotContain(oldPath, _sftp.Files.Keys);
    }

    [Theory]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已作廢)]
    public async Task UploadAttachment_簽核中已核准已作廢不可上傳且訊息帶目前狀態(ContractStatus status)
    {
        var id = await AddSupplierContractAsync();
        SetStatus(id, status);

        var result = await CreateService().UploadAttachment(NewAttachment(id));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
        Assert.Empty(_sftp.Files);
        Assert.Null(Load(id).AttachmentPath);
    }

    [Fact]
    public async Task UploadAttachment_找不到合約單回傳找不到資料()
    {
        var result = await CreateService().UploadAttachment(NewAttachment(99999));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Theory]
    [InlineData("contract.docx", "application/pdf")]
    [InlineData("contract.pdf", "image/png")]
    [InlineData("contract", "application/pdf")]
    public async Task UploadAttachment_副檔名或內容類型不是PDF被擋下(string fileName, string contentType)
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile(fileName: fileName, contentType: contentType)));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("PDF", result.ErrorMessage);
        Assert.Empty(_sftp.Files);
    }

    [Fact]
    public async Task UploadAttachment_只改副檔名的非PDF檔被擋下()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile("MZ 這不是 PDF"u8.ToArray())));

        Assert.False(result.IsSuccess);
        Assert.Contains("PDF", result.ErrorMessage);
        Assert.Empty(_sftp.Files);
    }

    [Fact]
    public async Task UploadAttachment_空檔被擋下()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile(Array.Empty<byte>())));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task UploadAttachment_超過10MB被擋下()
    {
        var id = await AddSupplierContractAsync();
        var big = new byte[10 * 1024 * 1024 + 1];
        PdfBytes.CopyTo(big, 0);

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile(big)));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("10 MB", result.ErrorMessage);
        Assert.Empty(_sftp.Files);
    }

    [Fact]
    public async Task UploadAttachment_剛好10MB可以()
    {
        var id = await AddSupplierContractAsync();
        var exact = new byte[10 * 1024 * 1024];
        PdfBytes.CopyTo(exact, 0);

        var result = await CreateService().UploadAttachment(NewAttachment(id, NewFile(exact)));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task UploadAttachment_SFTP失敗時不改動原本的掃描檔()
    {
        var id = await AddSupplierContractAsync();
        await CreateService().UploadAttachment(NewAttachment(id));
        var oldPath = Load(id).AttachmentPath!;
        _sftp.FailUpload = true;

        var result = await CreateService().UploadAttachment(NewAttachment(id));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.Exception, result.ErrorCode);
        Assert.Equal(oldPath, Load(id).AttachmentPath);
        Assert.Contains(oldPath, _sftp.Files.Keys);
    }

    [Fact]
    public async Task DownloadAttachment_回傳PDF內容與以單號命名的檔名()
    {
        var id = await AddSupplierContractAsync();
        await CreateService().UploadAttachment(NewAttachment(id));

        var result = await CreateService().DownloadAttachment(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal($"{Load(id).No}.pdf", result.Data!.FileName);
        using var buffer = new MemoryStream();
        await result.Data.Content.CopyToAsync(buffer);
        Assert.Equal(PdfBytes, buffer.ToArray());
    }

    [Fact]
    public async Task DownloadAttachment_尚未上傳時回傳找不到資料()
    {
        var id = await AddSupplierContractAsync();

        var result = await CreateService().DownloadAttachment(id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Contains("尚未上傳", result.ErrorMessage);
    }

    [Fact]
    public async Task DownloadAttachment_找不到合約單回傳找不到資料()
    {
        var result = await CreateService().DownloadAttachment(99999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task DownloadAttachment_遠端檔案不存在時原樣轉出錯誤()
    {
        var id = await AddSupplierContractAsync();
        await CreateService().UploadAttachment(NewAttachment(id));
        _sftp.Files.Clear();

        var result = await CreateService().DownloadAttachment(id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 簽核動作 ----------

    private static ApprovalVM Vm(string no) => new ApprovalVM { TableType = TableType.合約單, TableId = no };

    private async Task<string> AddDraftWithFileAsync(ContractAddVM? vm = null)
    {
        var added = await CreateService().Add(vm ?? NewSupplierContract());
        Assert.True(added.IsSuccess, added.ErrorMessage);
        var uploaded = await CreateService().UploadAttachment(NewAttachment(added.Data!.Id));
        Assert.True(uploaded.IsSuccess, uploaded.ErrorMessage);
        return added.Data.No;
    }

    private async Task<string> AddWithStatusAsync(ContractStatus status)
    {
        var no = await AddDraftWithFileAsync();
        SetStatus(Load(no).Id, status);
        return no;
    }

    private t_3050Contract Load(string no)
    {
        using var db = _testDb.CreateContext();
        return db.t_3050Contract.Single(x => x.No == no);
    }

    private static async Task WriteEngineMarkerAsync(ERP.EntityModels.Context.ERPDbContext db)
    {
        db.SystemConfig.Add(new SystemConfig { ConfigType = "EngineMarker", Name = "引擎紀錄", Sort = 0, IsActive = true });
        await db.SaveChangesAsync();
    }

    private bool EngineMarkerExists()
    {
        using var check = _testDb.CreateContext();
        return check.SystemConfig.Any(x => x.ConfigType == "EngineMarker");
    }

    [Fact]
    public async Task SendApproval_草稿送簽後變成簽核中()
    {
        var no = await AddDraftWithFileAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_客戶合約也能送簽()
    {
        var no = await AddDraftWithFileAsync(NewCustomerContract());

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_已退回可以重送()
    {
        var no = await AddWithStatusAsync(ContractStatus.已退回);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_沒有上傳掃描檔被擋下()
    {
        var added = await CreateService().Add(NewSupplierContract());

        var result = await CreateService().SendApproval(Vm(added.Data!.No));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("合約掃描檔", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.草稿, Load(added.Data.No).Status);
    }

    [Theory]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已作廢)]
    public async Task SendApproval_簽核中已核准已作廢不可送簽且訊息帶目前狀態(ContractStatus status)
    {
        var no = await AddWithStatusAsync(status);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
        Assert.Equal((int)status, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_找不到單號回傳找不到資料()
    {
        var result = await CreateService().SendApproval(Vm("CT0000000"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task SendApproval_簽約公司已停用被擋下()
    {
        var no = await AddDraftWithFileAsync();
        SetCompanyActive(_companyId, false);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("已停用", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.草稿, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_供應商已停用被擋下()
    {
        var no = await AddDraftWithFileAsync();
        SetSupplierActive(_supplierId, false);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("供應商", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.草稿, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_客戶合約不受供應商停用影響()
    {
        var no = await AddDraftWithFileAsync(NewCustomerContract());
        SetSupplierActive(_supplierId, false);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task SendApproval_沒有啟用的簽核設定時原樣轉出引擎錯誤()
    {
        var no = await AddDraftWithFileAsync();
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "找不到啟用中的簽核設定");

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal("找不到啟用中的簽核設定", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.草稿, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_有多組啟用的簽核設定時原樣轉出引擎錯誤()
    {
        var no = await AddDraftWithFileAsync();
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.MultipleApprovalSettingsExists);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.MultipleApprovalSettingsExists, result.ErrorCode);
        Assert.Equal((int)ContractStatus.草稿, Load(no).Status);
    }

    [Fact]
    public async Task SendApproval_引擎失敗時引擎寫入的紀錄與狀態一併回復()
    {
        var no = await AddDraftWithFileAsync();
        var service = CreateService(out var db);
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnSend = () => WriteEngineMarkerAsync(db);

        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)ContractStatus.草稿, Load(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task Approval_最後一關核准後變成已核准()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        _approval.ApprovalResult = ResultModel<string>.Ok(ApprovalWorkflowService.CompletedMessage);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已核准, Load(no).Status);
    }

    [Fact]
    public async Task Approval_中間關卡核准時維持簽核中()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        _approval.ApprovalResult = ResultModel<string>.Ok("已核准，等待下一關");

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Theory]
    [InlineData(ContractStatus.草稿)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已退回)]
    [InlineData(ContractStatus.已作廢)]
    public async Task Approval_非簽核中不可簽核且訊息帶目前狀態(ContractStatus status)
    {
        var no = await AddWithStatusAsync(status);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
    }

    [Fact]
    public async Task Approval_還沒輪到時原樣轉出引擎錯誤且狀態不變()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        _approval.ApprovalResult = ResultModel<string>.Error(ErrorCodeType.NotYetTurnForApprovalStep);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotYetTurnForApprovalStep, result.ErrorCode);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Fact]
    public async Task Approval_引擎失敗時引擎寫入的紀錄一併回復()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        var service = CreateService(out var db, OtherUserId);
        _approval.ApprovalResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnApproval = () => WriteEngineMarkerAsync(db);

        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task Approval_引擎成功時引擎寫入的紀錄與已核准狀態一併保留()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        var service = CreateService(out var db, OtherUserId);
        _approval.ApprovalResult = ResultModel<string>.Ok(ApprovalWorkflowService.CompletedMessage);
        _approval.OnApproval = () => WriteEngineMarkerAsync(db);

        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已核准, Load(no).Status);
        Assert.True(EngineMarkerExists());
    }

    [Fact]
    public async Task RejectApproval_拒絕後變成已退回且可重新編輯()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);

        var result = await CreateService(OtherUserId).RejectApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已退回, Load(no).Status);
        var vm = NewSupplierContract();
        vm.Id = Load(no).Id;
        vm.Amount = 1m;
        Assert.True((await CreateService().Edit(vm)).IsSuccess);
    }

    [Fact]
    public async Task RejectApproval_沒填原因時原樣轉出引擎錯誤且狀態不變()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        _approval.RejectResult = ResultModel<string>.Error(ErrorCodeType.IncompleteInfo, "請填寫駁回原因");

        var result = await CreateService(OtherUserId).RejectApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal("請填寫駁回原因", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Theory]
    [InlineData(ContractStatus.草稿)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已退回)]
    [InlineData(ContractStatus.已作廢)]
    public async Task RejectApproval_非簽核中不可拒絕且訊息帶目前狀態(ContractStatus status)
    {
        var no = await AddWithStatusAsync(status);

        var result = await CreateService(OtherUserId).RejectApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
    }

    [Fact]
    public async Task RejectApproval_引擎失敗時引擎寫入的紀錄一併回復()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        var service = CreateService(out var db, OtherUserId);
        _approval.RejectResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnReject = () => WriteEngineMarkerAsync(db);

        var result = await service.RejectApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task RevokeApproval_撤銷後變成已退回()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已退回, Load(no).Status);
    }

    [Fact]
    public async Task RevokeApproval_不是送簽人時原樣轉出引擎錯誤且狀態不變()
    {
        var no = await AddWithStatusAsync(ContractStatus.簽核中);
        _approval.RevokeResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "只能撤回自己送出的簽核");

        var result = await CreateService(OtherUserId).RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal("只能撤回自己送出的簽核", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.簽核中, Load(no).Status);
    }

    [Theory]
    [InlineData(ContractStatus.草稿)]
    [InlineData(ContractStatus.已核准)]
    [InlineData(ContractStatus.已退回)]
    [InlineData(ContractStatus.已作廢)]
    public async Task RevokeApproval_非簽核中不可撤銷且訊息帶目前狀態(ContractStatus status)
    {
        var no = await AddWithStatusAsync(status);

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
    }

    [Fact]
    public async Task VoidApproval_已核准作廢後變成已作廢且不可重送()
    {
        var no = await AddWithStatusAsync(ContractStatus.已核准);

        var result = await CreateService(OtherUserId).VoidApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已作廢, Load(no).Status);
        var resend = await CreateService().SendApproval(Vm(no));
        Assert.False(resend.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, resend.ErrorCode);
    }

    [Fact]
    public async Task VoidApproval_不是最後一關簽核人時原樣轉出引擎錯誤且狀態不變()
    {
        var no = await AddWithStatusAsync(ContractStatus.已核准);
        _approval.VoidResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "只能作廢自己的簽核");

        var result = await CreateService(OtherUserId).VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal("只能作廢自己的簽核", result.ErrorMessage);
        Assert.Equal((int)ContractStatus.已核准, Load(no).Status);
    }

    [Theory]
    [InlineData(ContractStatus.草稿)]
    [InlineData(ContractStatus.簽核中)]
    [InlineData(ContractStatus.已退回)]
    [InlineData(ContractStatus.已作廢)]
    public async Task VoidApproval_只有已核准可以作廢且訊息帶目前狀態(ContractStatus status)
    {
        var no = await AddWithStatusAsync(status);

        var result = await CreateService(OtherUserId).VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(status.ToString(), result.ErrorMessage);
    }

    [Fact]
    public async Task VoidApproval_作廢後合約編號可以讓新單沿用()
    {
        var no = await AddWithStatusAsync(ContractStatus.已核准);
        await CreateService(OtherUserId).VoidApproval(Vm(no));

        var reopened = await CreateService().Add(NewSupplierContract());

        Assert.True(reopened.IsSuccess, reopened.ErrorMessage);
    }

    [Fact]
    public async Task VoidApproval_引擎失敗時引擎寫入的紀錄一併回復()
    {
        var no = await AddWithStatusAsync(ContractStatus.已核准);
        var service = CreateService(out var db, OtherUserId);
        _approval.VoidResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnVoid = () => WriteEngineMarkerAsync(db);

        var result = await service.VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)ContractStatus.已核准, Load(no).Status);
        Assert.False(EngineMarkerExists());
    }

    // ---------- 可見範圍、篩選、有效性與動作旗標 ----------

    private const int ViewerRoleId = 2;
    private const int PlainRoleId = 3;
    private const int StrangerUserId = 30;
    private const int SignerUserId = 31;

    private void AddRecord(string no, int? userId = null, int? roleId = null, int stepOrder = 1,
        ApprovalStatus status = ApprovalStatus.Pending)
    {
        using var db = _testDb.CreateContext();
        db.ApprovalRecord.Add(new ApprovalRecord
        {
            ApprovalStepId = 0,
            TableId = no,
            TableType = (int)TableType.合約單,
            StepOrder = stepOrder,
            UserId = userId,
            RoleId = roleId,
            Status = (int)status
        });
        db.SaveChanges();
    }

    private void MakeMyTurn(string no) =>
        _approval.PendingNotify.Add(new GetApprovalNotifyVM { TableType = (int)TableType.合約單, TableId = no });

    private async Task<string> AddContractAsync(
        string contractNo, DateTime? start = null, DateTime? end = null, ContractStatus status = ContractStatus.草稿,
        ContractAddVM? template = null, int createdBy = CurrentUserId)
    {
        var vm = template ?? NewSupplierContract(contractNo);
        vm.ContractNo = contractNo;
        vm.StartDate = start ?? vm.StartDate;
        vm.EndDate = end ?? vm.EndDate;
        var added = await CreateService(createdBy).Add(vm);
        Assert.True(added.IsSuccess, added.ErrorMessage);
        if (status != ContractStatus.草稿)
        {
            SetStatus(added.Data!.Id, status);
        }
        return added.Data!.No;
    }

    private static IEnumerable<string?> Nos(ResultModel<PagedResult<ContractVM>> result) =>
        result.Data!.Items.Select(x => x.ContractNo);

    [Fact]
    public async Task 有檢視權限者看得到全部合約單()
    {
        GrantPermission(ViewerRoleId, PermissionType.合約單檢視);
        await AddContractAsync("BY-ME");
        await AddContractAsync("BY-OTHER", createdBy: OtherUserId);

        var result = await CreateService(StrangerUserId, ViewerRoleId).Index(new ContractSearchVM());

        Assert.Equal(2, result.Data!.TotalCount);
    }

    [Fact]
    public async Task 沒有檢視權限者只看得到自己建立的合約單()
    {
        await AddContractAsync("BY-ME");
        await AddContractAsync("BY-OTHER", createdBy: OtherUserId);

        var result = await CreateService(CurrentUserId, PlainRoleId).Index(new ContractSearchVM());

        Assert.Equal(new[] { "BY-ME" }, Nos(result).ToArray());
    }

    [Fact]
    public async Task 沒有檢視權限的簽核人看得到輪到自己簽核的合約單()
    {
        var no = await AddContractAsync("PENDING", status: ContractStatus.簽核中);
        await AddContractAsync("OTHER", status: ContractStatus.簽核中);
        MakeMyTurn(no);

        var result = await CreateService(SignerUserId, PlainRoleId).Index(new ContractSearchVM());

        Assert.Equal(new[] { "PENDING" }, Nos(result).ToArray());
    }

    [Fact]
    public async Task 簽核紀錄中有自己的簽核人仍看得到簽完的合約單()
    {
        var no = await AddContractAsync("SIGNED", status: ContractStatus.已核准);
        AddRecord(no, userId: SignerUserId, status: ApprovalStatus.Approved);

        var result = await CreateService(SignerUserId, PlainRoleId).Index(new ContractSearchVM());

        Assert.Equal(new[] { "SIGNED" }, Nos(result).ToArray());
    }

    [Fact]
    public async Task 角色簽核正在待簽的人看得到合約單但送簽紀錄不算()
    {
        var waiting = await AddContractAsync("ROLE-WAIT", status: ContractStatus.簽核中);
        var sendOnly = await AddContractAsync("ROLE-SEND", status: ContractStatus.簽核中);
        AddRecord(waiting, roleId: PlainRoleId, stepOrder: 1, status: ApprovalStatus.Pending);
        AddRecord(sendOnly, roleId: PlainRoleId, stepOrder: 0, status: ApprovalStatus.Pending);

        var result = await CreateService(SignerUserId, PlainRoleId).Index(new ContractSearchVM());

        Assert.Equal(new[] { "ROLE-WAIT" }, Nos(result).ToArray());
    }

    [Fact]
    public async Task 與合約單無關的人看不到單筆也下載不到掃描檔()
    {
        var no = await AddDraftWithFileAsync();
        var id = Load(no).Id;

        var get = await CreateService(StrangerUserId, PlainRoleId).Get(id);
        var download = await CreateService(StrangerUserId, PlainRoleId).DownloadAttachment(id);

        Assert.Equal(ErrorCodeType.NotFoundData, get.ErrorCode);
        Assert.Equal(ErrorCodeType.NotFoundData, download.ErrorCode);
    }

    [Fact]
    public async Task 沒有檢視權限的簽核人可以檢視並下載待簽核單的掃描檔()
    {
        var no = await AddDraftWithFileAsync();
        SetStatus(Load(no).Id, ContractStatus.簽核中);
        MakeMyTurn(no);
        var id = Load(no).Id;

        var get = await CreateService(SignerUserId, PlainRoleId).Get(id);
        var download = await CreateService(SignerUserId, PlainRoleId).DownloadAttachment(id);

        Assert.True(get.IsSuccess, get.ErrorMessage);
        Assert.True(download.IsSuccess, download.ErrorMessage);
    }

    [Fact]
    public async Task 篩選只縮小不擴大可見範圍()
    {
        await AddContractAsync("BY-OTHER", createdBy: OtherUserId);

        var result = await CreateService(CurrentUserId, PlainRoleId).Index(new ContractSearchVM { Keyword = "BY-OTHER" });

        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task 關鍵字比對單號與合約編號()
    {
        var no = await AddContractAsync("ABC-100");
        await AddContractAsync("XYZ-200");

        var byContractNo = await CreateService().Index(new ContractSearchVM { Keyword = "ABC" });
        var byNo = await CreateService().Index(new ContractSearchVM { Keyword = no });

        Assert.Equal(new[] { "ABC-100" }, Nos(byContractNo).ToArray());
        Assert.Equal(new[] { "ABC-100" }, Nos(byNo).ToArray());
    }

    [Fact]
    public async Task 可依類型對象簽約公司與狀態篩選()
    {
        var otherSupplier = AddSupplier("S002", "另一供應商");
        var otherCompany = AddCompany("乙公司", "11111111");
        await AddContractAsync("SUP-A");
        var template = NewSupplierContract();
        template.SupplierId = otherSupplier;
        template.CompanyId = otherCompany;
        await AddContractAsync("SUP-B", template: template, status: ContractStatus.已退回);
        await AddContractAsync("CUS-A", template: NewCustomerContract());

        Assert.Equal(new[] { "CUS-A" }, Nos(await CreateService().Index(new ContractSearchVM { Type = (int)ContractType.客戶合約 })).ToArray());
        Assert.Equal(new[] { "SUP-B" }, Nos(await CreateService().Index(new ContractSearchVM { SupplierId = otherSupplier })).ToArray());
        Assert.Equal(new[] { "CUS-A" }, Nos(await CreateService().Index(new ContractSearchVM { CustomerId = _customerId })).ToArray());
        Assert.Equal(new[] { "SUP-B" }, Nos(await CreateService().Index(new ContractSearchVM { CompanyId = otherCompany })).ToArray());
        Assert.Equal(new[] { "SUP-B" }, Nos(await CreateService().Index(new ContractSearchVM { Status = (int)ContractStatus.已退回 })).ToArray());
    }

    [Fact]
    public async Task 日期區間以與合約起訖有交集為準且邊界日算有交集()
    {
        await AddContractAsync("JUN", new DateTime(2026, 6, 1), new DateTime(2026, 6, 30));
        await AddContractAsync("JUL", new DateTime(2026, 7, 1), new DateTime(2026, 7, 31));

        var touchEnd = await CreateService().Index(new ContractSearchVM { PeriodStart = new DateTime(2026, 6, 30), PeriodEnd = new DateTime(2026, 6, 30) });
        var gap = await CreateService().Index(new ContractSearchVM { PeriodStart = new DateTime(2026, 8, 1), PeriodEnd = new DateTime(2026, 8, 31) });
        var across = await CreateService().Index(new ContractSearchVM { PeriodStart = new DateTime(2026, 6, 15), PeriodEnd = new DateTime(2026, 7, 15) });
        var openEnded = await CreateService().Index(new ContractSearchVM { PeriodStart = new DateTime(2026, 7, 15) });

        Assert.Equal(new[] { "JUN" }, Nos(touchEnd).ToArray());
        Assert.Empty(gap.Data!.Items);
        Assert.Equal(2, across.Data!.TotalCount);
        Assert.Equal(new[] { "JUL" }, Nos(openEnded).ToArray());
    }

    [Fact]
    public async Task 日期區間結束早於開始被擋下()
    {
        var result = await CreateService().Index(new ContractSearchVM
        {
            PeriodStart = new DateTime(2026, 7, 1),
            PeriodEnd = new DateTime(2026, 6, 1)
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task 待我簽核只列出輪到自己的簽核中合約單()
    {
        var mine = await AddContractAsync("MINE", status: ContractStatus.簽核中);
        await AddContractAsync("NOT-MINE", status: ContractStatus.簽核中);
        var staleNotify = await AddContractAsync("STALE", status: ContractStatus.已核准);
        MakeMyTurn(mine);
        MakeMyTurn(staleNotify);

        var result = await CreateService().Index(new ContractSearchVM { PendingMyApproval = true });

        Assert.Equal(new[] { "MINE" }, Nos(result).ToArray());
    }

    [Fact]
    public async Task 有效性以已核准且起訖日含當天計算()
    {
        var today = DateTime.Today;
        await AddContractAsync("START-TODAY", today, today.AddDays(30), ContractStatus.已核准);
        await AddContractAsync("END-TODAY", today.AddDays(-30), today, ContractStatus.已核准);
        await AddContractAsync("ENDED-YESTERDAY", today.AddDays(-30), today.AddDays(-1), ContractStatus.已核准);
        await AddContractAsync("STARTS-TOMORROW", today.AddDays(1), today.AddDays(30), ContractStatus.已核准);
        await AddContractAsync("DRAFT-IN-RANGE", today.AddDays(-1), today.AddDays(1));
        await AddContractAsync("VOID-IN-RANGE", today.AddDays(-1), today.AddDays(1), ContractStatus.已作廢);

        var all = (await CreateService().Index(new ContractSearchVM { PageSize = 50 })).Data!.Items
            .ToDictionary(x => x.ContractNo!);
        var effective = await CreateService().Index(new ContractSearchVM { Validity = ContractValidity.生效中 });
        var expired = await CreateService().Index(new ContractSearchVM { Validity = ContractValidity.已到期 });

        Assert.Equal("生效中", all["START-TODAY"].ValidityDisplay);
        Assert.Equal("生效中", all["END-TODAY"].ValidityDisplay);
        Assert.Equal("已到期", all["ENDED-YESTERDAY"].ValidityDisplay);
        Assert.Null(all["STARTS-TOMORROW"].Validity);
        Assert.Null(all["DRAFT-IN-RANGE"].Validity);
        Assert.Null(all["VOID-IN-RANGE"].Validity);
        Assert.Equal(new[] { "END-TODAY", "START-TODAY" }, Nos(effective).OrderBy(x => x).ToArray());
        Assert.Equal(new[] { "ENDED-YESTERDAY" }, Nos(expired).ToArray());
    }

    [Fact]
    public async Task 動作旗標依權限狀態與簽核紀錄由後端計算()
    {
        GrantPermission(ViewerRoleId, PermissionType.合約單檢視);
        GrantPermission(ViewerRoleId, PermissionType.合約單編輯);
        GrantPermission(ViewerRoleId, PermissionType.合約單刪除);
        await AddContractAsync("DRAFT");
        await AddContractAsync("RETURNED", status: ContractStatus.已退回);
        var inApproval = await AddContractAsync("IN-APPROVAL", status: ContractStatus.簽核中);
        var approved = await AddContractAsync("APPROVED", status: ContractStatus.已核准);
        AddRecord(inApproval, userId: CurrentUserId, stepOrder: 0, status: ApprovalStatus.Approved);
        AddRecord(inApproval, userId: SignerUserId, stepOrder: 1);
        AddRecord(approved, userId: CurrentUserId, stepOrder: 0, status: ApprovalStatus.Approved);
        AddRecord(approved, userId: SignerUserId, stepOrder: 1, status: ApprovalStatus.Approved);
        MakeMyTurn(inApproval);

        ContractVM Find(ResultModel<PagedResult<ContractVM>> r, string contractNo) =>
            r.Data!.Items.Single(x => x.ContractNo == contractNo);

        // 送簽人本人（有編輯與刪除權限）
        var asSender = await CreateService(CurrentUserId, ViewerRoleId).Index(new ContractSearchVM { PageSize = 50 });
        Assert.True(Find(asSender, "DRAFT").CanEdit);
        Assert.True(Find(asSender, "DRAFT").CanDelete);
        Assert.True(Find(asSender, "RETURNED").CanEdit);
        Assert.False(Find(asSender, "RETURNED").CanDelete);
        Assert.True(Find(asSender, "IN-APPROVAL").CanRevoke);
        Assert.False(Find(asSender, "IN-APPROVAL").CanEdit);
        Assert.True(Find(asSender, "IN-APPROVAL").CanApprove);
        Assert.False(Find(asSender, "APPROVED").CanVoid);

        // 最後一關簽核人：沒有編輯權限，只能作廢已核准的單
        var asSigner = await CreateService(SignerUserId, PlainRoleId).Index(new ContractSearchVM { PageSize = 50 });
        var signerApproved = Find(asSigner, "APPROVED");
        Assert.True(signerApproved.CanVoid);
        Assert.False(signerApproved.CanEdit);
        Assert.False(signerApproved.CanRevoke);
        Assert.False(Find(asSigner, "IN-APPROVAL").CanRevoke);
    }

    [Fact]
    public async Task 單筆檢視也帶動作旗標與有效性()
    {
        GrantPermission(ViewerRoleId, PermissionType.合約單檢視);
        GrantPermission(ViewerRoleId, PermissionType.合約單編輯);
        var today = DateTime.Today;
        var no = await AddContractAsync("ONE", today.AddDays(-1), today.AddDays(1));

        var result = await CreateService(CurrentUserId, ViewerRoleId).Get(Load(no).Id);

        Assert.True(result.Data!.CanEdit);
        Assert.False(result.Data.CanDelete);
        Assert.Null(result.Data.Validity);
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
