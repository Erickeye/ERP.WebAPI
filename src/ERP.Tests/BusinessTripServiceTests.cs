using ERP.Approval.Services;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.Enums.Login;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API;
using ERP.Service.API._1000Company;
using ERP.Tests.Support;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 出差單服務的行為測試（草稿階段）：建立、編輯、刪除、欄位驗證、單號、狀態限制、可見範圍與列表。
/// </summary>
public class BusinessTripServiceTests : IDisposable
{
    private const int CurrentUserId = 7;
    private const int OtherUserId = 8;
    private const int FirstApproverUserId = 21;
    private const int StrangerUserId = 22;

    private readonly TestDb _testDb = new();
    // 同一個測試內所有服務共用同一個流水號產生器，避免兩張單據拿到相同單號
    private readonly FakeSerialService _serial = new();
    private readonly FakeApprovalWorkflowService _approval = new();
    private readonly int _meStaffId;
    private readonly int _otherStaffId;
    private readonly int _resignedStaffId;
    private readonly int _customerId;

    public BusinessTripServiceTests()
    {
        using var db = _testDb.CreateContext();
        db.User.Add(NewUser(CurrentUserId, "me"));
        db.User.Add(NewUser(OtherUserId, "other"));
        var me = NewStaff("me", "我自己", resignationDate: null);
        var other = NewStaff("other", "同事", resignationDate: null);
        var resigned = NewStaff("resigned", "離職者", resignationDate: new DateTime(2025, 12, 31));
        var customer = NewCustomer("測試客戶");
        db.t_1000Staff.AddRange(me, other, resigned);
        db.t_2000Customer.Add(customer);
        db.SaveChanges();

        _meStaffId = me.StaffId;
        _otherStaffId = other.StaffId;
        _resignedStaffId = resigned.StaffId;
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

    private static t_1000Staff NewStaff(string account, string name, DateTime? resignationDate) => new t_1000Staff
    {
        ChineseName = name,
        Gender = 1,
        Account = account,
        ContactPhone = "0900000000",
        BankAccount = "000",
        ContactAddress = "測試地址",
        Email = $"{account}@example.com",
        Birthday = new DateTime(1990, 1, 1),
        HighestEducation = "大學",
        OnBoardDate = new DateTime(2020, 1, 1),
        ResignationDate = resignationDate,
        IdCard = $"ID-{account}",
        LaborPension = 0,
        BloodType = 1
    };

    private static t_2000Customer NewCustomer(string name) => new t_2000Customer
    {
        TaxInvoiceNumber = "12345678",
        Name = name,
        Owner = "負責人",
        RegisteredAddress = "登記地址"
    };

    private I_1060BusinessTripService CreateService(int userId = CurrentUserId, int roleId = 1)
    {
        var db = _testDb.CreateContext();
        return new _1060BusinessTripService(db, new FakeCurrentUser { UserId = userId, RoleId = roleId }, _serial, _approval);
    }

    private BusinessTripAddVM NewTrip(int? travelerStaffId = null) => new BusinessTripAddVM
    {
        TravelerStaffId = travelerStaffId ?? _meStaffId,
        Destination = "台中客戶端",
        Reason = "系統上線支援",
        StartDate = new DateTime(2026, 10, 5),
        EndDate = new DateTime(2026, 10, 6)
    };

    private t_1060BusinessTrip LoadTrip(string no)
    {
        using var db = _testDb.CreateContext();
        return db.t_1060BusinessTrip.Single(x => x.No == no);
    }

    // ---------- 建立 ----------

    [Fact]
    public async Task Add_建立草稿並帶入單號與建單人()
    {
        var service = CreateService();

        var result = await service.Add(NewTrip());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.StartsWith("BT", result.Data!.No);
        var trip = LoadTrip(result.Data.No);
        Assert.Equal((int)BusinessTripStatus.草稿, trip.Status);
        Assert.Equal(CurrentUserId, trip.CreateUserId);
        Assert.Equal(_meStaffId, trip.TravelerStaffId);
        Assert.Equal("台中客戶端", trip.Destination);
        Assert.Equal("系統上線支援", trip.Reason);
        Assert.Equal(new DateTime(2026, 10, 5), trip.StartDate);
        Assert.Equal(new DateTime(2026, 10, 6), trip.EndDate);
    }

    // ---------- 欄位驗證 ----------

    [Fact]
    public async Task Add_出差人已離職時被擋下()
    {
        var service = CreateService();

        var result = await service.Add(NewTrip(_resignedStaffId));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("出差人", result.ErrorMessage);
        using var db = _testDb.CreateContext();
        Assert.Empty(db.t_1060BusinessTrip);
    }

    [Fact]
    public async Task Add_出差人不存在時被擋下()
    {
        var service = CreateService();

        var result = await service.Add(NewTrip(99999));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
    }

    [Fact]
    public async Task Add_客戶不存在時被擋下()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.CustomerId = 99999;

        var result = await service.Add(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("客戶", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_客戶存在時可選填()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.CustomerId = _customerId;
        vm.Note = "需帶筆電";

        var result = await service.Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var trip = LoadTrip(result.Data!.No);
        Assert.Equal(_customerId, trip.CustomerId);
        Assert.Equal("需帶筆電", trip.Note);
    }

    [Fact]
    public async Task Add_迄日早於起日時被擋下並說明原因()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.StartDate = new DateTime(2026, 10, 6);
        vm.EndDate = new DateTime(2026, 10, 5);

        var result = await service.Add(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("迄日不得早於起日", result.ErrorMessage);
    }

    [Fact]
    public async Task Add_起迄同一天可建立()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.StartDate = new DateTime(2026, 10, 5);
        vm.EndDate = new DateTime(2026, 10, 5);

        var result = await service.Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Add_起日不設下限可補單()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.StartDate = new DateTime(2020, 1, 1);
        vm.EndDate = new DateTime(2020, 1, 2);

        var result = await service.Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Theory]
    [InlineData("", "系統上線支援", "出差地點")]
    [InlineData("   ", "系統上線支援", "出差地點")]
    [InlineData("台中客戶端", "", "出差事由")]
    [InlineData("台中客戶端", "  ", "出差事由")]
    public async Task Add_地點或事由空白時被擋下(string destination, string reason, string expectedField)
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.Destination = destination;
        vm.Reason = reason;

        var result = await service.Add(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains(expectedField, result.ErrorMessage);
    }

    [Fact]
    public async Task Add_地點與事由前後空白會被去除()
    {
        var service = CreateService();
        var vm = NewTrip();
        vm.Destination = "  台中客戶端 ";
        vm.Reason = " 系統上線支援  ";

        var result = await service.Add(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var trip = LoadTrip(result.Data!.No);
        Assert.Equal("台中客戶端", trip.Destination);
        Assert.Equal("系統上線支援", trip.Reason);
    }

    // ---------- 單號 ----------

    [Fact]
    public async Task Add_連續建立兩張單號不重複()
    {
        var service = CreateService();

        var first = await service.Add(NewTrip());
        var second = await service.Add(NewTrip());

        Assert.True(first.IsSuccess, first.ErrorMessage);
        Assert.True(second.IsSuccess, second.ErrorMessage);
        Assert.NotEqual(first.Data!.No, second.Data!.No);
    }

    [Fact]
    public async Task Add_單號為BT加民國年月加三位流水()
    {
        // 使用真正的流水號服務，確認前綴與格式
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = CurrentUserId }, new SerialService(db), _approval);
        var now = DateTime.Now;

        var first = await service.Add(NewTrip());
        var second = await service.Add(NewTrip());

        var rocYearMonth = $"{now.Year - 1911:D2}{now.Month:D2}";
        Assert.Equal($"BT{rocYearMonth}001", first.Data!.No);
        Assert.Equal($"BT{rocYearMonth}002", second.Data!.No);
    }

    // ---------- 編輯 ----------

    private async Task<(int Id, string No)> AddTripAsync(int userId = CurrentUserId, int? travelerStaffId = null)
    {
        var result = await CreateService(userId).Add(NewTrip(travelerStaffId));
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return (result.Data!.Id, result.Data.No);
    }

    /// <summary>直接種入一筆簽核紀錄（引擎的紀錄，用來驗證簽核人的可見範圍）。</summary>
    private void SeedApprovalRecord(
        string no,
        int? userId = null,
        int? roleId = null,
        int stepOrder = 1,
        ApprovalStatus status = ApprovalStatus.Pending)
    {
        using var db = _testDb.CreateContext();
        db.ApprovalRecord.Add(new ApprovalRecord
        {
            ApprovalStepId = 0,
            TableId = no,
            TableType = (int)TableType.出差單,
            StepOrder = stepOrder,
            UserId = userId,
            RoleId = roleId,
            Status = (int)status
        });
        db.SaveChanges();
    }

    private void SetStatus(string no, BusinessTripStatus status)
    {
        using var db = _testDb.CreateContext();
        db.t_1060BusinessTrip.Single(x => x.No == no).Status = (int)status;
        db.SaveChanges();
    }

    [Fact]
    public async Task Edit_草稿可修改欄位且單號與建單人不變()
    {
        var (id, no) = await AddTripAsync();
        var vm = NewTrip(_otherStaffId);
        vm.Id = id;
        vm.Destination = "高雄分公司";
        vm.Reason = "教育訓練";
        vm.StartDate = new DateTime(2026, 11, 1);
        vm.EndDate = new DateTime(2026, 11, 3);
        vm.CustomerId = _customerId;
        vm.Note = "改期";

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(no, result.Data!.No);
        var trip = LoadTrip(no);
        Assert.Equal(_otherStaffId, trip.TravelerStaffId);
        Assert.Equal("高雄分公司", trip.Destination);
        Assert.Equal("教育訓練", trip.Reason);
        Assert.Equal(new DateTime(2026, 11, 1), trip.StartDate);
        Assert.Equal(new DateTime(2026, 11, 3), trip.EndDate);
        Assert.Equal(_customerId, trip.CustomerId);
        Assert.Equal("改期", trip.Note);
        Assert.Equal(CurrentUserId, trip.CreateUserId);
        Assert.Equal((int)BusinessTripStatus.草稿, trip.Status);
    }

    [Fact]
    public async Task Edit_已退回可修改且狀態維持已退回()
    {
        var (id, no) = await AddTripAsync();
        SetStatus(no, BusinessTripStatus.已退回);
        var vm = NewTrip();
        vm.Id = id;
        vm.Destination = "新竹客戶端";

        var result = await CreateService().Edit(vm);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var trip = LoadTrip(no);
        Assert.Equal("新竹客戶端", trip.Destination);
        Assert.Equal((int)BusinessTripStatus.已退回, trip.Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.簽核中, "簽核中")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task Edit_簽核中_已核准_已作廢不可編輯且訊息含目前狀態(BusinessTripStatus status, string statusText)
    {
        var (id, no) = await AddTripAsync();
        SetStatus(no, status);
        var vm = NewTrip();
        vm.Id = id;
        vm.Destination = "不該被存入";

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(statusText, result.ErrorMessage);
        Assert.Equal("台中客戶端", LoadTrip(no).Destination);
    }

    [Fact]
    public async Task Edit_找不到出差單時回傳找不到資料()
    {
        var vm = NewTrip();
        vm.Id = 99999;

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Edit_欄位驗證失敗時不更動資料()
    {
        var (id, no) = await AddTripAsync();
        var vm = NewTrip(_resignedStaffId);
        vm.Id = id;
        vm.Destination = "不該被存入";

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        var trip = LoadTrip(no);
        Assert.Equal(_meStaffId, trip.TravelerStaffId);
        Assert.Equal("台中客戶端", trip.Destination);
    }

    // ---------- 刪除 ----------

    [Fact]
    public async Task Delete_草稿可刪除()
    {
        var (id, no) = await AddTripAsync();

        var result = await CreateService().Delete(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        using var db = _testDb.CreateContext();
        Assert.DoesNotContain(db.t_1060BusinessTrip, x => x.No == no);
    }

    [Theory]
    [InlineData(BusinessTripStatus.簽核中, "簽核中")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已退回, "已退回")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task Delete_非草稿不可刪除且訊息含目前狀態(BusinessTripStatus status, string statusText)
    {
        var (id, no) = await AddTripAsync();
        SetStatus(no, status);

        var result = await CreateService().Delete(id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains(statusText, result.ErrorMessage);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task Delete_找不到出差單時回傳找不到資料()
    {
        var result = await CreateService().Delete(99999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 單筆與出差天數 ----------

    private async Task<int> AddTripOnAsync(DateTime start, DateTime end, int userId = CurrentUserId, int? travelerStaffId = null)
    {
        var vm = NewTrip(travelerStaffId);
        vm.StartDate = start;
        vm.EndDate = end;
        var result = await CreateService(userId).Add(vm);
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return result.Data!.Id;
    }

    [Fact]
    public async Task Get_回傳完整內容與出差人客戶名稱和狀態顯示()
    {
        var vm = NewTrip(_otherStaffId);
        vm.CustomerId = _customerId;
        vm.Note = "備註內容";
        var added = await CreateService().Add(vm);

        var result = await CreateService().Get(added.Data!.Id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var data = result.Data!;
        Assert.Equal(added.Data.No, data.No);
        Assert.Equal(_otherStaffId, data.TravelerStaffId);
        Assert.Equal("同事", data.TravelerName);
        Assert.Equal("台中客戶端", data.Destination);
        Assert.Equal("系統上線支援", data.Reason);
        Assert.Equal(_customerId, data.CustomerId);
        Assert.Equal("測試客戶", data.CustomerName);
        Assert.Equal("備註內容", data.Note);
        Assert.Equal((int)BusinessTripStatus.草稿, data.Status);
        Assert.Equal("草稿", data.StatusDisplay);
        Assert.Equal(CurrentUserId, data.CreateUserId);
        Assert.Equal("me", data.CreateUserName);
    }

    [Theory]
    [InlineData("2026-10-05", "2026-10-05", 1)]
    [InlineData("2026-10-05", "2026-10-06", 2)]
    [InlineData("2026-10-30", "2026-11-02", 4)]
    [InlineData("2026-12-31", "2027-01-01", 2)]
    public async Task Get_出差天數為迄日減起日加一(string start, string end, int expectedDays)
    {
        var id = await AddTripOnAsync(DateTime.Parse(start), DateTime.Parse(end));

        var result = await CreateService().Get(id);

        Assert.Equal(expectedDays, result.Data!.Days);
    }

    [Fact]
    public async Task Get_找不到出差單時回傳找不到資料()
    {
        var result = await CreateService().Get(99999);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 可見範圍與列表 ----------

    [Fact]
    public async Task Index_只列出建單人或出差人是自己的出差單()
    {
        var ownByMe = await AddTripOnAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1));
        var createdForOther = await AddTripOnAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), travelerStaffId: _otherStaffId);
        var othersCreatedForMe = await AddTripOnAsync(new DateTime(2026, 10, 3), new DateTime(2026, 10, 3), userId: OtherUserId, travelerStaffId: _meStaffId);
        var unrelated = await AddTripOnAsync(new DateTime(2026, 10, 4), new DateTime(2026, 10, 4), userId: OtherUserId, travelerStaffId: _otherStaffId);

        var result = await CreateService().Index(new BusinessTripSearchVM());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var ids = result.Data!.Items.Select(x => x.Id).ToList();
        Assert.Contains(ownByMe, ids);
        Assert.Contains(createdForOther, ids);
        Assert.Contains(othersCreatedForMe, ids);
        Assert.DoesNotContain(unrelated, ids);
        Assert.Equal(3, result.Data.TotalCount);
    }

    [Fact]
    public async Task Index_沒有對應員工的使用者只看到自己建立的出差單()
    {
        // 使用者 9 沒有帳號相同的員工
        using (var db = _testDb.CreateContext())
        {
            db.User.Add(NewUser(9, "nostaff"));
            db.SaveChanges();
        }
        var mine = await AddTripOnAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), userId: 9, travelerStaffId: _otherStaffId);
        await AddTripOnAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), userId: CurrentUserId, travelerStaffId: _meStaffId);

        var result = await CreateService(userId: 9).Index(new BusinessTripSearchVM());

        var item = Assert.Single(result.Data!.Items);
        Assert.Equal(mine, item.Id);
    }

    [Fact]
    public async Task Index_預設依出差起日由新到舊()
    {
        var older = await AddTripOnAsync(new DateTime(2026, 9, 1), new DateTime(2026, 9, 2));
        var newest = await AddTripOnAsync(new DateTime(2026, 11, 1), new DateTime(2026, 11, 2));
        var middle = await AddTripOnAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 2));

        var result = await CreateService().Index(new BusinessTripSearchVM());

        Assert.Equal(new[] { newest, middle, older }, result.Data!.Items.Select(x => x.Id).ToArray());
    }

    [Fact]
    public async Task Index_可指定排序欄位覆蓋預設排序()
    {
        var older = await AddTripOnAsync(new DateTime(2026, 9, 1), new DateTime(2026, 9, 2));
        var newest = await AddTripOnAsync(new DateTime(2026, 11, 1), new DateTime(2026, 11, 2));

        var result = await CreateService().Index(new BusinessTripSearchVM { SortColumn = "StartDate", SortDirection = "asc" });

        Assert.Equal(new[] { older, newest }, result.Data!.Items.Select(x => x.Id).ToArray());
    }

    [Fact]
    public async Task Index_依頁數與每頁筆數分頁()
    {
        for (var day = 1; day <= 5; day++)
        {
            await AddTripOnAsync(new DateTime(2026, 10, day), new DateTime(2026, 10, day));
        }

        var page2 = await CreateService().Index(new BusinessTripSearchVM { PageNumber = 2, PageSize = 2 });

        Assert.Equal(5, page2.Data!.TotalCount);
        Assert.Equal(2, page2.Data.PageNumber);
        Assert.Equal(2, page2.Data.Items.Count);
        // 起日由新到舊：第 1 頁是 10/5、10/4，第 2 頁是 10/3、10/2
        Assert.Equal(
            new[] { new DateTime(2026, 10, 3), new DateTime(2026, 10, 2) },
            page2.Data.Items.Select(x => x.StartDate).ToArray());
    }

    [Fact]
    public async Task Index_列表項目帶出天數出差人與狀態顯示()
    {
        await AddTripOnAsync(new DateTime(2026, 10, 5), new DateTime(2026, 10, 7));

        var result = await CreateService().Index(new BusinessTripSearchVM());

        var item = Assert.Single(result.Data!.Items);
        Assert.Equal(3, item.Days);
        Assert.Equal("我自己", item.TravelerName);
        Assert.Equal("草稿", item.StatusDisplay);
    }

    [Fact]
    public async Task Index_排序欄位若是計算欄位不會出錯()
    {
        await AddTripOnAsync(new DateTime(2026, 10, 5), new DateTime(2026, 10, 7));

        var result = await CreateService().Index(new BusinessTripSearchVM { SortColumn = "Days" });

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Single(result.Data!.Items);
    }

    [Fact]
    public async Task Get_看不到與自己無關的出差單()
    {
        var unrelated = await AddTripOnAsync(new DateTime(2026, 10, 4), new DateTime(2026, 10, 4), userId: OtherUserId, travelerStaffId: _otherStaffId);

        var result = await CreateService().Get(unrelated);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Get_出差人本人可以看到別人代填的出差單()
    {
        var id = await AddTripOnAsync(new DateTime(2026, 10, 3), new DateTime(2026, 10, 3), userId: OtherUserId, travelerStaffId: _meStaffId);

        var result = await CreateService().Get(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task Edit_不能編輯與自己無關的出差單()
    {
        var (id, no) = await AddTripAsync(userId: OtherUserId, travelerStaffId: _otherStaffId);
        var vm = NewTrip();
        vm.Id = id;
        vm.Destination = "不該被存入";

        var result = await CreateService().Edit(vm);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal("台中客戶端", LoadTrip(no).Destination);
    }

    [Fact]
    public async Task Delete_不能刪除與自己無關的出差單()
    {
        var (id, no) = await AddTripAsync(userId: OtherUserId, travelerStaffId: _otherStaffId);

        var result = await CreateService().Delete(id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(no).Status);
    }

    // ---------- 目前使用者對應的員工 ----------

    [Fact]
    public async Task GetCurrentStaff_回傳帳號相同的員工()
    {
        var result = await CreateService().GetCurrentStaff();

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(_meStaffId, result.Data!.Value);
        Assert.Equal("我自己", result.Data.Text);
    }

    [Fact]
    public async Task GetCurrentStaff_沒有對應員工時回傳找不到資料()
    {
        using (var db = _testDb.CreateContext())
        {
            db.User.Add(NewUser(9, "nostaff"));
            db.SaveChanges();
        }

        var result = await CreateService(userId: 9).GetCurrentStaff();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task GetCurrentStaff_對應員工已離職時回傳找不到資料()
    {
        using (var db = _testDb.CreateContext())
        {
            db.User.Add(NewUser(10, "resigned"));
            db.SaveChanges();
        }

        var result = await CreateService(userId: 10).GetCurrentStaff();

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    // ---------- 送簽 ----------

    private static ApprovalVM Vm(string no) => new ApprovalVM { TableId = no };

    private async Task<string> AddDraftAsync(int? travelerStaffId = null, DateTime? start = null, DateTime? end = null)
    {
        var vm = NewTrip(travelerStaffId);
        vm.StartDate = start ?? vm.StartDate;
        vm.EndDate = end ?? vm.EndDate;
        var result = await CreateService().Add(vm);
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return result.Data!.No;
    }

    [Fact]
    public async Task SendApproval_草稿送簽後變成簽核中()
    {
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_已退回的出差單可重新送簽()
    {
        var no = await AddDraftAsync();
        SetStatus(no, BusinessTripStatus.已退回);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.簽核中, "簽核中")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task SendApproval_狀態不符時被擋下並說明目前狀態(BusinessTripStatus status, string display)
    {
        var no = await AddDraftAsync();
        SetStatus(no, status);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains($"【{display}】", result.ErrorMessage);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_找不到單號時回找不到資料()
    {
        var result = await CreateService().SendApproval(Vm("BT0000000"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task SendApproval_看不到的出差單視為找不到()
    {
        var no = await AddDraftAsync();

        // 其他使用者既不是建單人，出差人也不是他
        var result = await CreateService(OtherUserId).SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_引擎失敗時狀態維持草稿且錯誤原樣轉出()
    {
        var no = await AddDraftAsync();
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "找不到啟用中的簽核設定");

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal("找不到啟用中的簽核設定", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_多組啟用設定的錯誤原樣轉出()
    {
        var no = await AddDraftAsync();
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.MultipleApprovalSettingsExists);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.MultipleApprovalSettingsExists, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_引擎失敗時引擎已寫入的紀錄一併回滾_已退回維持已退回()
    {
        var no = await AddDraftAsync();
        SetStatus(no, BusinessTripStatus.已退回);
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = CurrentUserId }, _serial, _approval);
        // 模擬簽核引擎在失敗前已自行寫入一筆紀錄，驗證會連它一起回滾
        _approval.SendResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnSend = async () =>
        {
            db.SystemConfig.Add(new SystemConfig { ConfigType = "EngineMarker", Name = "引擎紀錄", Sort = 0, IsActive = true });
            await db.SaveChangesAsync();
        };

        var result = await service.SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);
        using var check = _testDb.CreateContext();
        Assert.False(check.SystemConfig.Any(x => x.ConfigType == "EngineMarker"));
    }

    // ---------- 送簽時的重疊檢查（本單：10/5～10/6） ----------

    private async Task<string> AddOtherTripAsync(
        BusinessTripStatus status,
        DateTime start,
        DateTime end,
        int? travelerStaffId = null)
    {
        var no = await AddDraftAsync(travelerStaffId, start, end);
        SetStatus(no, status);
        return no;
    }

    [Theory]
    [InlineData(BusinessTripStatus.簽核中)]
    [InlineData(BusinessTripStatus.已核准)]
    public async Task SendApproval_與有效出差單日期重疊時被擋下並列出單號與日期(BusinessTripStatus status)
    {
        var conflictNo = await AddOtherTripAsync(status, new DateTime(2026, 10, 6), new DateTime(2026, 10, 8));
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains(conflictNo, result.ErrorMessage);
        Assert.Contains("2026/10/06", result.ErrorMessage);
        Assert.Contains("2026/10/08", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_重疊被擋下時不會呼叫簽核引擎()
    {
        await AddOtherTripAsync(BusinessTripStatus.已核准, new DateTime(2026, 10, 5), new DateTime(2026, 10, 5));
        var no = await AddDraftAsync();
        var engineCalled = false;
        _approval.OnSend = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.False(engineCalled);
    }

    [Theory]
    [InlineData(2026, 10, 3, 2026, 10, 4, false)] // 相鄰日（前一天結束）不算重疊
    [InlineData(2026, 10, 7, 2026, 10, 8, false)] // 相鄰日（後一天開始）不算重疊
    [InlineData(2026, 10, 3, 2026, 10, 5, true)]  // 對方迄日等於本單起日，同日算重疊
    [InlineData(2026, 10, 6, 2026, 10, 8, true)]  // 對方起日等於本單迄日，同日算重疊
    [InlineData(2026, 10, 1, 2026, 10, 30, true)] // 對方包住本單
    [InlineData(2026, 10, 5, 2026, 10, 6, true)]  // 完全相同
    public async Task SendApproval_日期交集邊界(int y1, int m1, int d1, int y2, int m2, int d2, bool conflict)
    {
        await AddOtherTripAsync(BusinessTripStatus.簽核中, new DateTime(y1, m1, d1), new DateTime(y2, m2, d2));
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.Equal(!conflict, result.IsSuccess);
        Assert.Equal(conflict ? BusinessTripStatus.草稿 : BusinessTripStatus.簽核中, (BusinessTripStatus)LoadTrip(no).Status);
    }

    [Fact]
    public async Task SendApproval_本單被包住的單日出差單也算重疊()
    {
        await AddOtherTripAsync(BusinessTripStatus.簽核中, new DateTime(2026, 10, 6), new DateTime(2026, 10, 6));
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿)]
    [InlineData(BusinessTripStatus.已退回)]
    [InlineData(BusinessTripStatus.已作廢)]
    public async Task SendApproval_草稿_已退回_已作廢不參與重疊檢查(BusinessTripStatus status)
    {
        await AddOtherTripAsync(status, new DateTime(2026, 10, 5), new DateTime(2026, 10, 6));
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task SendApproval_不同出差人的重疊不算衝突()
    {
        await AddOtherTripAsync(BusinessTripStatus.已核准, new DateTime(2026, 10, 5), new DateTime(2026, 10, 6), _otherStaffId);
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task SendApproval_多張衝突的出差單全部列出()
    {
        var first = await AddOtherTripAsync(BusinessTripStatus.簽核中, new DateTime(2026, 10, 4), new DateTime(2026, 10, 5));
        var second = await AddOtherTripAsync(BusinessTripStatus.已核准, new DateTime(2026, 10, 6), new DateTime(2026, 10, 7));
        var no = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Contains(first, result.ErrorMessage);
        Assert.Contains(second, result.ErrorMessage);
    }

    [Fact]
    public async Task SendApproval_已退回的單重送時不會與自己衝突()
    {
        var no = await AddDraftAsync();
        SetStatus(no, BusinessTripStatus.已退回);
        // 另一張同期間但已退回的單據不影響
        await AddOtherTripAsync(BusinessTripStatus.已退回, new DateTime(2026, 10, 5), new DateTime(2026, 10, 6));

        var result = await CreateService().SendApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    [Fact]
    public async Task SendApproval_重疊檢查涵蓋看不到的他人出差單()
    {
        // 目前使用者代同事填單；衝突的單據由同事自己建立，目前使用者既非建單人也非出差人，看不到它，但重疊檢查不受可見範圍限制
        var conflictNo = await AddOtherTripByAsync(OtherUserId, _otherStaffId, BusinessTripStatus.已核准);
        var no = await AddDraftAsync(_otherStaffId);
        Assert.False((await CreateService().Get(LoadTrip(conflictNo).Id)).IsSuccess);

        var result = await CreateService().SendApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Contains(conflictNo, result.ErrorMessage);
    }

    private async Task<string> AddOtherTripByAsync(int createUserId, int travelerStaffId, BusinessTripStatus status)
    {
        var added = await CreateService(createUserId).Add(NewTrip(travelerStaffId));
        Assert.True(added.IsSuccess, added.ErrorMessage);
        SetStatus(added.Data!.No, status);
        return added.Data.No;
    }

    // ---------- 核准 ----------

    /// <summary>建立一張簽核中的出差單；簽核人（OtherUserId）既不是建單人也不是出差人。</summary>
    private async Task<string> AddInApprovalAsync()
    {
        var no = await AddDraftAsync();
        SetStatus(no, BusinessTripStatus.簽核中);
        return no;
    }

    /// <summary>讓引擎的待簽核查詢回報：目前登入者輪到簽這張出差單。</summary>
    private void MakeMyTurn(string no)
    {
        _approval.PendingNotify.Add(new GetApprovalNotifyVM { TableType = (int)TableType.出差單, TableId = no });
    }

    /// <summary>模擬簽核引擎在動作中已自行寫入一筆紀錄，用來驗證失敗時會連它一起回滾。</summary>
    private static async Task WriteEngineMarkerAsync(ERPDbContext db)
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
    public async Task Approval_最後一關核准後變成已核准()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        _approval.ApprovalResult = ResultModel<string>.Ok(ApprovalWorkflowService.CompletedMessage);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData("簽核成功，已進入下一關")]
    [InlineData("簽核成功，等待同層其他簽核人員")]
    public async Task Approval_中間關卡或同關尚有其他人未核准時維持簽核中(string engineMessage)
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        _approval.ApprovalResult = ResultModel<string>.Ok(engineMessage);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(engineMessage, result.Data);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task Approval_還沒輪到時引擎的錯誤原樣轉出且狀態不變()
    {
        var no = await AddInApprovalAsync();
        SeedApprovalRecord(no, userId: OtherUserId, stepOrder: 2, status: ApprovalStatus.Pending);
        _approval.ApprovalResult = ResultModel<string>.Error(ErrorCodeType.NotYetTurnForApprovalStep);

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotYetTurnForApprovalStep, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿, "草稿")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已退回, "已退回")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task Approval_狀態不是簽核中時被擋下並說明目前狀態且不呼叫引擎(BusinessTripStatus status, string display)
    {
        var no = await AddDraftAsync();
        SetStatus(no, status);
        MakeMyTurn(no);
        var engineCalled = false;
        _approval.OnApproval = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains($"【{display}】", result.ErrorMessage);
        Assert.False(engineCalled);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task Approval_找不到單號時回找不到資料()
    {
        var result = await CreateService(OtherUserId).Approval(Vm("BT0000000"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Approval_與出差單無關的使用者視為找不到()
    {
        var no = await AddInApprovalAsync();

        var result = await CreateService(OtherUserId).Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task Approval_引擎失敗時引擎已寫入的紀錄一併回滾且狀態維持簽核中()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = OtherUserId }, _serial, _approval);
        _approval.ApprovalResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnApproval = () => WriteEngineMarkerAsync(db);

        var result = await service.Approval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal("引擎失敗", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task Approval_引擎成功時引擎寫入的紀錄與已核准狀態一併保留()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = OtherUserId }, _serial, _approval);
        _approval.OnApproval = () => WriteEngineMarkerAsync(db);

        var result = await service.Approval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(no).Status);
        Assert.True(EngineMarkerExists());
    }

    // ---------- 拒絕 ----------

    [Fact]
    public async Task RejectApproval_拒絕後變成已退回()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);

        var result = await CreateService(OtherUserId).RejectApproval(new ApprovalVM { TableId = no, Memo = "行程不合理" });

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RejectApproval_未填原因時引擎的錯誤原樣轉出且狀態不變()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        _approval.RejectResult = ResultModel<string>.Error(ErrorCodeType.IncompleteInfo, "請填寫駁回原因");

        var result = await CreateService(OtherUserId).RejectApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.IncompleteInfo, result.ErrorCode);
        Assert.Equal("請填寫駁回原因", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RejectApproval_還沒輪到時引擎的錯誤原樣轉出且狀態不變()
    {
        var no = await AddInApprovalAsync();
        SeedApprovalRecord(no, userId: OtherUserId, stepOrder: 2, status: ApprovalStatus.Pending);
        _approval.RejectResult = ResultModel<string>.Error(ErrorCodeType.NotYetTurnForApprovalStep);

        var result = await CreateService(OtherUserId).RejectApproval(new ApprovalVM { TableId = no, Memo = "原因" });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotYetTurnForApprovalStep, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿, "草稿")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已退回, "已退回")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task RejectApproval_狀態不是簽核中時被擋下並說明目前狀態且不呼叫引擎(BusinessTripStatus status, string display)
    {
        var no = await AddDraftAsync();
        SetStatus(no, status);
        MakeMyTurn(no);
        var engineCalled = false;
        _approval.OnReject = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService(OtherUserId).RejectApproval(new ApprovalVM { TableId = no, Memo = "原因" });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains($"【{display}】", result.ErrorMessage);
        Assert.False(engineCalled);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RejectApproval_與出差單無關的使用者視為找不到()
    {
        var no = await AddInApprovalAsync();

        var result = await CreateService(OtherUserId).RejectApproval(new ApprovalVM { TableId = no, Memo = "原因" });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RejectApproval_引擎失敗時引擎已寫入的紀錄一併回滾且狀態維持簽核中()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = OtherUserId }, _serial, _approval);
        _approval.RejectResult = ResultModel<string>.Error(ErrorCodeType.InvalidApproval, "引擎失敗");
        _approval.OnReject = () => WriteEngineMarkerAsync(db);

        var result = await service.RejectApproval(new ApprovalVM { TableId = no, Memo = "原因" });

        Assert.False(result.IsSuccess);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
        Assert.False(EngineMarkerExists());
    }

    // ---------- 簽核人的檢視範圍 ----------

    [Fact]
    public async Task Get_輪到自己簽核的人即使不是建單人或出差人也看得到()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        var id = LoadTrip(no).Id;

        var result = await CreateService(OtherUserId).Get(id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(no, result.Data!.No);
    }

    [Fact]
    public async Task Get_簽核紀錄中有自己的人看得到_包含還沒輪到與已簽過()
    {
        var notYetTurn = await AddInApprovalAsync();
        SeedApprovalRecord(notYetTurn, userId: OtherUserId, stepOrder: 2, status: ApprovalStatus.Pending);
        var signed = await AddInApprovalAsync();
        SeedApprovalRecord(signed, userId: OtherUserId, stepOrder: 1, status: ApprovalStatus.Approved);

        var first = await CreateService(OtherUserId).Get(LoadTrip(notYetTurn).Id);
        var second = await CreateService(OtherUserId).Get(LoadTrip(signed).Id);

        Assert.True(first.IsSuccess, first.ErrorMessage);
        Assert.True(second.IsSuccess, second.ErrorMessage);
        Assert.False(first.Data!.CanApprove);
        Assert.False(second.Data!.CanApprove);
    }

    [Fact]
    public async Task Get_指定角色簽核時該角色的人在待簽期間看得到()
    {
        const int signerRoleId = 5;
        var no = await AddInApprovalAsync();
        SeedApprovalRecord(no, roleId: signerRoleId, stepOrder: 1, status: ApprovalStatus.Pending);

        var sameRole = await CreateService(OtherUserId, roleId: signerRoleId).Get(LoadTrip(no).Id);
        var otherRole = await CreateService(OtherUserId, roleId: 6).Get(LoadTrip(no).Id);

        Assert.True(sameRole.IsSuccess, sameRole.ErrorMessage);
        Assert.False(otherRole.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, otherRole.ErrorCode);
    }

    [Fact]
    public async Task Get_送簽人的角色不會讓同角色的其他人看到出差單()
    {
        var no = await AddInApprovalAsync();
        // 引擎送簽時建立的第 0 筆紀錄帶送簽人的角色，且已是核准狀態
        SeedApprovalRecord(no, userId: CurrentUserId, roleId: 1, stepOrder: 0, status: ApprovalStatus.Approved);

        var result = await CreateService(OtherUserId, roleId: 1).Get(LoadTrip(no).Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task Get_別張出差單的簽核紀錄不影響這張的可見範圍()
    {
        var other = await AddInApprovalAsync();
        var mine = await AddInApprovalAsync();
        SeedApprovalRecord(other, userId: OtherUserId, stepOrder: 1);

        var result = await CreateService(OtherUserId).Get(LoadTrip(mine).Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task 簽核人可以檢視但不能編輯_刪除或送簽()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        var id = LoadTrip(no).Id;
        var editVm = NewTrip();
        editVm.Id = id;

        var edit = await CreateService(OtherUserId).Edit(editVm);
        var delete = await CreateService(OtherUserId).Delete(id);
        var send = await CreateService(OtherUserId).SendApproval(Vm(no));

        Assert.Equal(ErrorCodeType.NotFoundData, edit.ErrorCode);
        Assert.Equal(ErrorCodeType.NotFoundData, delete.ErrorCode);
        Assert.Equal(ErrorCodeType.NotFoundData, send.ErrorCode);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    // ---------- 待我簽核 ----------

    [Fact]
    public async Task Index_待自己簽核的出差單也在列表內_即使不是建單人或出差人()
    {
        var pendingForMe = await AddInApprovalAsync();
        var unrelated = await AddInApprovalAsync();
        MakeMyTurn(pendingForMe);

        var result = await CreateService(OtherUserId).Index(new BusinessTripSearchVM());

        Assert.True(result.IsSuccess, result.ErrorMessage);
        var ids = result.Data!.Items.Select(x => x.Id).ToList();
        Assert.Equal(new[] { LoadTrip(pendingForMe).Id }, ids);
    }

    [Fact]
    public async Task Index_待我簽核篩選只留下輪到自己簽的出差單_自己建立的其他單不列出()
    {
        var pendingForMe = await AddInApprovalAsync();
        var ownDraft = await AddDraftAsync();
        var ownInApprovalNotMyTurn = await AddInApprovalAsync();
        MakeMyTurn(pendingForMe);

        var result = await CreateService(CurrentUserId).Index(new BusinessTripSearchVM { PendingMyApproval = true });

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal(new[] { pendingForMe }, result.Data!.Items.Select(x => x.No).ToList());
        Assert.Equal(1, result.Data.TotalCount);
    }

    [Fact]
    public async Task Index_待我簽核篩選不會把其他單據類型的待簽核算進來()
    {
        var no = await AddInApprovalAsync();
        // 領料單剛好有相同的單號字串，也不能被當成出差單
        _approval.PendingNotify.Add(new GetApprovalNotifyVM { TableType = (int)TableType.領料單, TableId = no });

        var result = await CreateService(CurrentUserId).Index(new BusinessTripSearchVM { PendingMyApproval = true });

        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task Index_待我簽核篩選時單據已不是簽核中就不列出()
    {
        var no = await AddInApprovalAsync();
        SetStatus(no, BusinessTripStatus.已退回);
        MakeMyTurn(no);

        var result = await CreateService(CurrentUserId).Index(new BusinessTripSearchVM { PendingMyApproval = true });

        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task Index_沒有待簽核時待我簽核篩選回傳空列表()
    {
        await AddInApprovalAsync();

        var result = await CreateService(CurrentUserId).Index(new BusinessTripSearchVM { PendingMyApproval = true });

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Empty(result.Data!.Items);
    }

    [Fact]
    public async Task Get_輪到自己簽核時CanApprove為true_否則為false()
    {
        var myTurn = await AddInApprovalAsync();
        var notMyTurn = await AddInApprovalAsync();
        MakeMyTurn(myTurn);

        var turn = await CreateService().Get(LoadTrip(myTurn).Id);
        var other = await CreateService().Get(LoadTrip(notMyTurn).Id);

        Assert.True(turn.Data!.CanApprove);
        Assert.False(other.Data!.CanApprove);
    }

    [Fact]
    public async Task Get_單據不是簽核中時即使在待簽核查詢內CanApprove仍為false()
    {
        var no = await AddInApprovalAsync();
        SetStatus(no, BusinessTripStatus.已核准);
        MakeMyTurn(no);

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.False(result.Data!.CanApprove);
    }

    [Fact]
    public async Task Index_列表項目帶出CanApprove()
    {
        var myTurn = await AddInApprovalAsync();
        var notMyTurn = await AddInApprovalAsync();
        MakeMyTurn(myTurn);

        var result = await CreateService().Index(new BusinessTripSearchVM());

        var items = result.Data!.Items.ToDictionary(x => x.No!);
        Assert.True(items[myTurn].CanApprove);
        Assert.False(items[notMyTurn].CanApprove);
    }

    [Fact]
    public async Task Index_排序欄位若是CanApprove不會出錯()
    {
        await AddInApprovalAsync();

        var result = await CreateService().Index(new BusinessTripSearchVM { SortColumn = "CanApprove" });

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    // ---------- 撤銷 ----------

    [Fact]
    public async Task RevokeApproval_簽核中撤銷後變成已退回()
    {
        var no = await AddInApprovalAsync();

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RevokeApproval_出差人本人也在範圍內可撤銷()
    {
        // 由其他人代填、出差人是我
        var added = await CreateService(OtherUserId).Add(NewTrip(_meStaffId));
        var no = added.Data!.No;
        SetStatus(no, BusinessTripStatus.簽核中);

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿, "草稿")]
    [InlineData(BusinessTripStatus.已核准, "已核准")]
    [InlineData(BusinessTripStatus.已退回, "已退回")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task RevokeApproval_狀態不是簽核中時被擋下並說明目前狀態且不呼叫引擎(BusinessTripStatus status, string display)
    {
        var no = await AddDraftAsync();
        SetStatus(no, status);
        var engineCalled = false;
        _approval.OnRevoke = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains($"【{display}】", result.ErrorMessage);
        Assert.False(engineCalled);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RevokeApproval_建單人與出差人以外的使用者視為找不到且不呼叫引擎()
    {
        var no = await AddInApprovalAsync();
        // 簽核人可以檢視這張單，但撤銷只限建單人或出差人
        MakeMyTurn(no);
        var engineCalled = false;
        _approval.OnRevoke = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService(OtherUserId).RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.False(engineCalled);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RevokeApproval_找不到單號時回找不到資料()
    {
        var result = await CreateService().RevokeApproval(Vm("BT0000000"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task RevokeApproval_不是送簽人時引擎的錯誤原樣轉出且狀態不變()
    {
        var no = await AddInApprovalAsync();
        _approval.RevokeResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "只能撤回自己送出的簽核");

        var result = await CreateService().RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal("只能撤回自己送出的簽核", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task RevokeApproval_引擎失敗時引擎已寫入的紀錄一併回滾且狀態維持簽核中()
    {
        var no = await AddInApprovalAsync();
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = CurrentUserId }, _serial, _approval);
        _approval.RevokeResult = ResultModel<string>.Error(ErrorCodeType.InvalidUserOperation, "目前狀態不可撤回簽核");
        _approval.OnRevoke = () => WriteEngineMarkerAsync(db);

        var result = await service.RevokeApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task RevokeApproval_引擎成功時引擎寫入的紀錄與已退回狀態一併保留()
    {
        var no = await AddInApprovalAsync();
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = CurrentUserId }, _serial, _approval);
        _approval.OnRevoke = () => WriteEngineMarkerAsync(db);

        var result = await service.RevokeApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);
        Assert.True(EngineMarkerExists());
    }

    // ---------- 已退回後編輯與重送 ----------

    [Fact]
    public async Task 撤銷後可編輯並重新送簽()
    {
        var no = await AddInApprovalAsync();
        Assert.True((await CreateService().RevokeApproval(Vm(no))).IsSuccess);
        var edit = NewTrip();
        edit.Id = LoadTrip(no).Id;
        edit.Destination = "高雄客戶端";

        var edited = await CreateService().Edit(edit);
        var sent = await CreateService().SendApproval(Vm(no));

        Assert.True(edited.IsSuccess, edited.ErrorMessage);
        Assert.True(sent.IsSuccess, sent.ErrorMessage);
        var trip = LoadTrip(no);
        Assert.Equal("高雄客戶端", trip.Destination);
        Assert.Equal((int)BusinessTripStatus.簽核中, trip.Status);
    }

    [Fact]
    public async Task 拒絕後可編輯並重新送簽()
    {
        var no = await AddInApprovalAsync();
        MakeMyTurn(no);
        Assert.True((await CreateService(OtherUserId).RejectApproval(new ApprovalVM { TableId = no, Memo = "原因" })).IsSuccess);
        var edit = NewTrip();
        edit.Id = LoadTrip(no).Id;
        edit.Reason = "補充說明後重送";

        var edited = await CreateService().Edit(edit);
        var sent = await CreateService().SendApproval(Vm(no));

        Assert.True(edited.IsSuccess, edited.ErrorMessage);
        Assert.True(sent.IsSuccess, sent.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    [Fact]
    public async Task 已退回的單重送時重新做重疊檢查()
    {
        var no = await AddInApprovalAsync();
        Assert.True((await CreateService().RevokeApproval(Vm(no))).IsSuccess);
        // 撤銷期間，同一位出差人又有一張同日期的有效出差單
        var otherNo = await AddOtherTripAsync(BusinessTripStatus.簽核中, new DateTime(2026, 10, 5), new DateTime(2026, 10, 6));

        var blocked = await CreateService().SendApproval(Vm(no));

        Assert.False(blocked.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, blocked.ErrorCode);
        Assert.Contains(otherNo, blocked.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已退回, LoadTrip(no).Status);

        // 對方作廢後不再占用，可重送
        SetStatus(otherNo, BusinessTripStatus.已作廢);
        var sent = await CreateService().SendApproval(Vm(no));
        Assert.True(sent.IsSuccess, sent.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(no).Status);
    }

    // ---------- CanEdit、CanRevoke（由後端判斷，前端不自行推算） ----------

    /// <summary>種入一輪送簽的紀錄：先是送簽人那筆（StepOrder 為 0），再接各關待簽或已簽的紀錄。</summary>
    private void SeedRound(string no, int senderUserId, params (int UserId, int StepOrder, ApprovalStatus Status)[] steps)
    {
        SeedApprovalRecord(no, userId: senderUserId, roleId: 1, stepOrder: 0, status: ApprovalStatus.Approved);
        foreach (var step in steps)
        {
            SeedApprovalRecord(no, userId: step.UserId, stepOrder: step.StepOrder, status: step.Status);
        }
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿, true)]
    [InlineData(BusinessTripStatus.簽核中, false)]
    [InlineData(BusinessTripStatus.已核准, false)]
    [InlineData(BusinessTripStatus.已退回, true)]
    [InlineData(BusinessTripStatus.已作廢, false)]
    public async Task Get_CanEdit依單據狀態決定(BusinessTripStatus status, bool expected)
    {
        var no = await AddDraftAsync();
        SetStatus(no, status);

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.Equal(expected, result.Data!.CanEdit);
    }

    [Fact]
    public async Task Get_簽核人看已退回的單且不是建單人或出差人時CanEdit為false()
    {
        var no = await AddDraftAsync();
        SeedRound(no, CurrentUserId, (OtherUserId, 1, ApprovalStatus.Rejected));
        SetStatus(no, BusinessTripStatus.已退回);

        var result = await CreateService(OtherUserId).Get(LoadTrip(no).Id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(result.Data!.CanEdit);
    }

    [Fact]
    public async Task Get_出差人本人看別人代填的已退回單時CanEdit為true()
    {
        var added = await CreateService(OtherUserId).Add(NewTrip(_meStaffId));
        SetStatus(added.Data!.No, BusinessTripStatus.已退回);

        var result = await CreateService().Get(added.Data.Id);

        Assert.True(result.Data!.CanEdit);
    }

    [Fact]
    public async Task Get_簽核中且目前使用者是送簽人時CanRevoke為true()
    {
        var no = await AddInApprovalAsync();
        SeedRound(no, CurrentUserId, (OtherUserId, 1, ApprovalStatus.Pending));

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.True(result.Data!.CanRevoke);
    }

    [Fact]
    public async Task Get_出差人不是送簽人時CanRevoke為false_因為引擎只允許送簽人撤銷()
    {
        var added = await CreateService(OtherUserId).Add(NewTrip(_meStaffId));
        var no = added.Data!.No;
        SetStatus(no, BusinessTripStatus.簽核中);
        SeedRound(no, OtherUserId, (OtherUserId, 1, ApprovalStatus.Pending));

        var result = await CreateService().Get(added.Data.Id);

        Assert.False(result.Data!.CanRevoke);
    }

    [Fact]
    public async Task Get_簽核人不是建單人或出差人時CanRevoke為false()
    {
        var no = await AddInApprovalAsync();
        SeedRound(no, CurrentUserId, (OtherUserId, 1, ApprovalStatus.Pending));

        var result = await CreateService(OtherUserId).Get(LoadTrip(no).Id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(result.Data!.CanRevoke);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿)]
    [InlineData(BusinessTripStatus.已核准)]
    [InlineData(BusinessTripStatus.已退回)]
    [InlineData(BusinessTripStatus.已作廢)]
    public async Task Get_狀態不是簽核中時CanRevoke為false(BusinessTripStatus status)
    {
        var no = await AddDraftAsync();
        SeedRound(no, CurrentUserId, (OtherUserId, 1, ApprovalStatus.Pending));
        SetStatus(no, status);

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.False(result.Data!.CanRevoke);
    }

    [Fact]
    public async Task Get_重送後以最近一輪的送簽人判斷CanRevoke()
    {
        var no = await AddInApprovalAsync();
        // 第一輪由我送簽、被退回；第二輪由別人（出差人以外的代送）送簽
        SeedRound(no, CurrentUserId, (OtherUserId, 1, ApprovalStatus.GetRejected));
        SeedRound(no, OtherUserId, (OtherUserId, 1, ApprovalStatus.Pending));

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.False(result.Data!.CanRevoke);
    }

    [Fact]
    public async Task Get_簽核中但沒有送簽紀錄時CanRevoke為false()
    {
        var no = await AddInApprovalAsync();

        var result = await CreateService().Get(LoadTrip(no).Id);

        Assert.False(result.Data!.CanRevoke);
    }

    [Fact]
    public async Task Index_列表項目帶出CanEdit與CanRevoke()
    {
        var draft = await AddDraftAsync();
        var inApproval = await AddInApprovalAsync();
        SeedRound(inApproval, CurrentUserId, (OtherUserId, 1, ApprovalStatus.Pending));

        var result = await CreateService().Index(new BusinessTripSearchVM());

        var items = result.Data!.Items.ToDictionary(x => x.No!);
        Assert.True(items[draft].CanEdit);
        Assert.False(items[draft].CanRevoke);
        Assert.False(items[inApproval].CanEdit);
        Assert.True(items[inApproval].CanRevoke);
    }

    [Theory]
    [InlineData("CanEdit")]
    [InlineData("CanRevoke")]
    [InlineData("CanVoid")]
    public async Task Index_排序欄位若是權限旗標不會出錯(string column)
    {
        await AddInApprovalAsync();

        var result = await CreateService().Index(new BusinessTripSearchVM { SortColumn = column });

        Assert.True(result.IsSuccess, result.ErrorMessage);
    }

    // ---------- 作廢 ----------

    /// <summary>建立一張已核准的出差單；最後一關簽核人是 OtherUserId（既不是建單人也不是出差人）。</summary>
    private async Task<string> AddApprovedAsync()
    {
        var no = await AddDraftAsync();
        SeedRound(no, CurrentUserId,
            (FirstApproverUserId, 1, ApprovalStatus.Approved),
            (OtherUserId, 2, ApprovalStatus.Approved));
        SetStatus(no, BusinessTripStatus.已核准);
        return no;
    }

    [Fact]
    public async Task VoidApproval_已核准作廢後變成已作廢()
    {
        var no = await AddApprovedAsync();

        var result = await CreateService(OtherUserId).VoidApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已作廢, LoadTrip(no).Status);
    }

    [Theory]
    [InlineData(BusinessTripStatus.草稿, "草稿")]
    [InlineData(BusinessTripStatus.簽核中, "簽核中")]
    [InlineData(BusinessTripStatus.已退回, "已退回")]
    [InlineData(BusinessTripStatus.已作廢, "已作廢")]
    public async Task VoidApproval_狀態不是已核准時被擋下並說明目前狀態且不呼叫引擎(BusinessTripStatus status, string display)
    {
        var no = await AddApprovedAsync();
        SetStatus(no, status);
        var engineCalled = false;
        _approval.OnVoid = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService(OtherUserId).VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, result.ErrorCode);
        Assert.Contains($"【{display}】", result.ErrorMessage);
        Assert.False(engineCalled);
        Assert.Equal((int)status, LoadTrip(no).Status);
    }

    [Fact]
    public async Task VoidApproval_找不到單號時回找不到資料()
    {
        var result = await CreateService(OtherUserId).VoidApproval(Vm("BT0000000"));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task VoidApproval_與出差單無關的使用者視為找不到且不呼叫引擎()
    {
        var no = await AddApprovedAsync();
        var engineCalled = false;
        _approval.OnVoid = () =>
        {
            engineCalled = true;
            return Task.CompletedTask;
        };

        var result = await CreateService(StrangerUserId).VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.False(engineCalled);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(no).Status);
    }

    [Fact]
    public async Task VoidApproval_不是最後一關簽核人時引擎的錯誤原樣轉出且狀態不變()
    {
        var no = await AddApprovedAsync();
        _approval.VoidResult = ResultModel<string>.Error(ErrorCodeType.NotFoundData, "只能作廢自己的簽核");

        // 第一關簽核人看得到這張單，但不是最後一關簽核人
        var result = await CreateService(FirstApproverUserId).VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
        Assert.Equal("只能作廢自己的簽核", result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(no).Status);
    }

    [Fact]
    public async Task VoidApproval_引擎失敗時引擎已寫入的紀錄一併回滾且狀態維持已核准()
    {
        var no = await AddApprovedAsync();
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = OtherUserId }, _serial, _approval);
        _approval.VoidResult = ResultModel<string>.Error(ErrorCodeType.InvalidUserOperation, "簽核尚未全部通過，不可作廢");
        _approval.OnVoid = () => WriteEngineMarkerAsync(db);

        var result = await service.VoidApproval(Vm(no));

        Assert.False(result.IsSuccess);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(no).Status);
        Assert.False(EngineMarkerExists());
    }

    [Fact]
    public async Task VoidApproval_引擎成功時引擎寫入的紀錄與已作廢狀態一併保留()
    {
        var no = await AddApprovedAsync();
        var db = _testDb.CreateContext();
        var service = new _1060BusinessTripService(db, new FakeCurrentUser { UserId = OtherUserId }, _serial, _approval);
        _approval.OnVoid = () => WriteEngineMarkerAsync(db);

        var result = await service.VoidApproval(Vm(no));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已作廢, LoadTrip(no).Status);
        Assert.True(EngineMarkerExists());
    }

    [Fact]
    public async Task 作廢後不可編輯_刪除_重送()
    {
        var no = await AddApprovedAsync();
        Assert.True((await CreateService(OtherUserId).VoidApproval(Vm(no))).IsSuccess);
        var id = LoadTrip(no).Id;
        var editVm = NewTrip();
        editVm.Id = id;

        var edit = await CreateService().Edit(editVm);
        var delete = await CreateService().Delete(id);
        var send = await CreateService().SendApproval(Vm(no));

        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, edit.ErrorCode);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, delete.ErrorCode);
        Assert.Equal(ErrorCodeType.InvalidDocumentStatus, send.ErrorCode);
        Assert.Contains("【已作廢】", send.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.已作廢, LoadTrip(no).Status);
    }

    [Fact]
    public async Task 作廢後同一出差人可另開同日期的新單並送簽()
    {
        var voided = await AddApprovedAsync();
        Assert.True((await CreateService(OtherUserId).VoidApproval(Vm(voided))).IsSuccess);
        var newNo = await AddDraftAsync();

        var result = await CreateService().SendApproval(Vm(newNo));

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(newNo).Status);
    }

    // ---------- CanVoid ----------

    [Fact]
    public async Task Get_已核准且是最後一關簽核人時CanVoid為true_第一關簽核人與建單人為false()
    {
        var no = await AddApprovedAsync();
        var id = LoadTrip(no).Id;

        var last = await CreateService(OtherUserId).Get(id);
        var first = await CreateService(FirstApproverUserId).Get(id);
        var creator = await CreateService().Get(id);

        Assert.True(last.Data!.CanVoid);
        Assert.False(first.Data!.CanVoid);
        Assert.False(creator.Data!.CanVoid);
    }

    [Theory]
    [InlineData(BusinessTripStatus.簽核中)]
    [InlineData(BusinessTripStatus.已退回)]
    [InlineData(BusinessTripStatus.已作廢)]
    public async Task Get_狀態不是已核准時CanVoid為false(BusinessTripStatus status)
    {
        var no = await AddApprovedAsync();
        SetStatus(no, status);

        var result = await CreateService(OtherUserId).Get(LoadTrip(no).Id);

        Assert.True(result.IsSuccess, result.ErrorMessage);
        Assert.False(result.Data!.CanVoid);
    }

    [Fact]
    public async Task Get_指定角色簽核時核准者的使用者識別碼被記在最後一關紀錄上_該人CanVoid為true()
    {
        var no = await AddDraftAsync();
        // 角色簽核的待簽紀錄沒有 UserId，核准後引擎記錄實際簽核人
        SeedApprovalRecord(no, userId: CurrentUserId, roleId: 1, stepOrder: 0, status: ApprovalStatus.Approved);
        SeedApprovalRecord(no, userId: OtherUserId, roleId: 5, stepOrder: 1, status: ApprovalStatus.Approved);
        SetStatus(no, BusinessTripStatus.已核准);

        var result = await CreateService(OtherUserId, roleId: 5).Get(LoadTrip(no).Id);

        Assert.True(result.Data!.CanVoid);
    }

    [Fact]
    public async Task Get_重送後只看最近一輪最後一關的簽核人_舊一輪的簽核人CanVoid為false()
    {
        var no = await AddDraftAsync();
        // 第一輪：最後一關是 OtherUserId，被拒絕；第二輪：最後一關是 FirstApproverUserId，全部核准
        SeedRound(no, CurrentUserId,
            (OtherUserId, 1, ApprovalStatus.Rejected),
            (FirstApproverUserId, 2, ApprovalStatus.GetRejected));
        SeedRound(no, CurrentUserId,
            (OtherUserId, 1, ApprovalStatus.Approved),
            (FirstApproverUserId, 2, ApprovalStatus.Approved));
        SetStatus(no, BusinessTripStatus.已核准);
        var id = LoadTrip(no).Id;

        var oldRoundSigner = await CreateService(OtherUserId).Get(id);
        var lastSigner = await CreateService(FirstApproverUserId).Get(id);

        Assert.False(oldRoundSigner.Data!.CanVoid);
        Assert.True(lastSigner.Data!.CanVoid);
    }

    [Fact]
    public async Task Index_列表項目帶出CanVoid()
    {
        var approved = await AddApprovedAsync();
        var inApproval = await AddInApprovalAsync();

        var result = await CreateService(OtherUserId).Index(new BusinessTripSearchVM());

        var items = result.Data!.Items.ToDictionary(x => x.No!);
        Assert.True(items[approved].CanVoid);
        Assert.DoesNotContain(inApproval, items.Keys);
    }

    // ---------- 列表篩選 ----------

    /// <summary>建立一張出差單並指定狀態，回傳單號；預設建單人為自己。</summary>
    private async Task<string> AddFilterTripAsync(
        DateTime start,
        DateTime end,
        BusinessTripStatus status = BusinessTripStatus.草稿,
        int userId = CurrentUserId,
        int? travelerStaffId = null)
    {
        var id = await AddTripOnAsync(start, end, userId, travelerStaffId);
        using var db = _testDb.CreateContext();
        var trip = db.t_1060BusinessTrip.Single(x => x.Id == id);
        trip.Status = (int)status;
        db.SaveChanges();
        return trip.No;
    }

    private async Task<List<string>> IndexNosAsync(BusinessTripSearchVM vm, int userId = CurrentUserId, int roleId = 1)
    {
        var result = await CreateService(userId, roleId).Index(vm);
        Assert.True(result.IsSuccess, result.ErrorMessage);
        return result.Data!.Items.Select(x => x.No!).OrderBy(x => x).ToList();
    }

    [Fact]
    public async Task Index_單號關鍵字做包含比對()
    {
        var first = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1));
        var second = await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2));

        // 只取單號尾碼，確認不是整串比對
        var keyword = second.Substring(second.Length - 2);
        var nos = await IndexNosAsync(new BusinessTripSearchVM { No = keyword });

        Assert.Equal(new[] { second }, nos);
        Assert.NotEqual(first, second);
    }

    [Fact]
    public async Task Index_單號關鍵字全空白視為沒有條件()
    {
        await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1));
        await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2));

        var nos = await IndexNosAsync(new BusinessTripSearchVM { No = "  " });

        Assert.Equal(2, nos.Count);
    }

    [Fact]
    public async Task Index_依出差人篩選()
    {
        var forMe = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1));
        var forOther = await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), travelerStaffId: _otherStaffId);

        var nos = await IndexNosAsync(new BusinessTripSearchVM { TravelerStaffId = _otherStaffId });

        Assert.Equal(new[] { forOther }, nos);
        Assert.NotEqual(forMe, forOther);
    }

    [Fact]
    public async Task Index_依出差單狀態篩選()
    {
        await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), BusinessTripStatus.草稿);
        var approved = await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), BusinessTripStatus.已核准);
        await AddFilterTripAsync(new DateTime(2026, 10, 3), new DateTime(2026, 10, 3), BusinessTripStatus.已退回);

        var nos = await IndexNosAsync(new BusinessTripSearchVM { Status = (int)BusinessTripStatus.已核准 });

        Assert.Equal(new[] { approved }, nos);
    }

    [Fact]
    public async Task Index_狀態篩選可選草稿_草稿的值是零不會被當成沒有條件()
    {
        var draft = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), BusinessTripStatus.草稿);
        await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), BusinessTripStatus.已核准);

        var nos = await IndexNosAsync(new BusinessTripSearchVM { Status = (int)BusinessTripStatus.草稿 });

        Assert.Equal(new[] { draft }, nos);
    }

    [Fact]
    public async Task Index_篩選條件可組合_必須全部符合()
    {
        await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), BusinessTripStatus.已核准, travelerStaffId: _meStaffId);
        await AddFilterTripAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 2), BusinessTripStatus.草稿, travelerStaffId: _otherStaffId);
        var target = await AddFilterTripAsync(new DateTime(2026, 10, 3), new DateTime(2026, 10, 3), BusinessTripStatus.已核准, travelerStaffId: _otherStaffId);

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            TravelerStaffId = _otherStaffId,
            Status = (int)BusinessTripStatus.已核准
        });

        Assert.Equal(new[] { target }, nos);
    }

    [Fact]
    public async Task Index_篩選條件符合時分頁總筆數也只算符合的()
    {
        for (var i = 1; i <= 3; i++)
        {
            await AddFilterTripAsync(new DateTime(2026, 10, i), new DateTime(2026, 10, i), BusinessTripStatus.已核准);
        }
        await AddFilterTripAsync(new DateTime(2026, 10, 9), new DateTime(2026, 10, 9), BusinessTripStatus.草稿);

        var result = await CreateService().Index(new BusinessTripSearchVM
        {
            Status = (int)BusinessTripStatus.已核准,
            PageNumber = 1,
            PageSize = 2
        });

        Assert.Equal(3, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);
    }

    [Fact]
    public async Task Index_篩選不會擴大可見範圍_出差人條件指到別人的單也看不到()
    {
        var unrelated = await AddFilterTripAsync(
            new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), userId: OtherUserId, travelerStaffId: _otherStaffId);

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            TravelerStaffId = _otherStaffId,
            No = unrelated,
            Status = (int)BusinessTripStatus.草稿,
            PeriodStart = new DateTime(2026, 9, 1),
            PeriodEnd = new DateTime(2026, 11, 1)
        });

        Assert.Empty(nos);
    }

    [Fact]
    public async Task Index_待我簽核可與其他篩選並用()
    {
        var matched = await AddFilterTripAsync(
            new DateTime(2026, 10, 5), new DateTime(2026, 10, 6), BusinessTripStatus.簽核中, userId: OtherUserId, travelerStaffId: _otherStaffId);
        var wrongTraveler = await AddFilterTripAsync(
            new DateTime(2026, 10, 5), new DateTime(2026, 10, 6), BusinessTripStatus.簽核中, userId: OtherUserId, travelerStaffId: _meStaffId);
        MakeMyTurn(matched);
        MakeMyTurn(wrongTraveler);

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            PendingMyApproval = true,
            TravelerStaffId = _otherStaffId,
            PeriodStart = new DateTime(2026, 10, 6),
            PeriodEnd = new DateTime(2026, 10, 6)
        });

        Assert.Equal(new[] { matched }, nos);
    }

    // 出差單為 10/10～10/12；區間「與出差起訖有交集」才算符合，邊界日算有交集
    [Theory]
    [InlineData("2026-10-01", "2026-10-09", false)] // 區間迄日是出差起日的前一天
    [InlineData("2026-10-13", "2026-10-20", false)] // 區間起日是出差迄日的後一天
    [InlineData("2026-10-01", "2026-10-10", true)]  // 區間迄日等於出差起日
    [InlineData("2026-10-12", "2026-10-20", true)]  // 區間起日等於出差迄日
    [InlineData("2026-10-01", "2026-10-30", true)]  // 區間完全包住出差
    [InlineData("2026-10-11", "2026-10-11", true)]  // 區間被出差包住
    [InlineData("2026-10-10", "2026-10-12", true)]  // 區間與出差完全相同
    [InlineData("2026-10-08", "2026-10-11", true)]  // 只重疊前半
    [InlineData("2026-10-11", "2026-10-15", true)]  // 只重疊後半
    public async Task Index_日期區間以與出差起訖有交集為準(string periodStart, string periodEnd, bool expected)
    {
        var no = await AddFilterTripAsync(new DateTime(2026, 10, 10), new DateTime(2026, 10, 12));

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            PeriodStart = DateTime.Parse(periodStart),
            PeriodEnd = DateTime.Parse(periodEnd)
        });

        Assert.Equal(expected ? new[] { no } : Array.Empty<string>(), nos);
    }

    [Fact]
    public async Task Index_日期區間不是只比對起日_區間落在出差中間也找得到()
    {
        // 起日 10/1 不在區間內，但出差一路到 10/20，區間 10/10～10/11 仍與它有交集
        var no = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 20));

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            PeriodStart = new DateTime(2026, 10, 10),
            PeriodEnd = new DateTime(2026, 10, 11)
        });

        Assert.Equal(new[] { no }, nos);
    }

    [Fact]
    public async Task Index_只填區間開始時列出迄日不早於它的出差單()
    {
        await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 9));
        var boundary = await AddFilterTripAsync(new DateTime(2026, 10, 5), new DateTime(2026, 10, 10));
        var later = await AddFilterTripAsync(new DateTime(2026, 10, 20), new DateTime(2026, 10, 21));

        var nos = await IndexNosAsync(new BusinessTripSearchVM { PeriodStart = new DateTime(2026, 10, 10) });

        Assert.Equal(new[] { boundary, later }.OrderBy(x => x), nos);
    }

    [Fact]
    public async Task Index_只填區間結束時列出起日不晚於它的出差單()
    {
        var early = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 2));
        var boundary = await AddFilterTripAsync(new DateTime(2026, 10, 10), new DateTime(2026, 10, 12));
        await AddFilterTripAsync(new DateTime(2026, 10, 11), new DateTime(2026, 10, 12));

        var nos = await IndexNosAsync(new BusinessTripSearchVM { PeriodEnd = new DateTime(2026, 10, 10) });

        Assert.Equal(new[] { early, boundary }.OrderBy(x => x), nos);
    }

    [Fact]
    public async Task Index_區間結束帶有時間時仍以日期為準_當天算有交集()
    {
        var no = await AddFilterTripAsync(new DateTime(2026, 10, 10), new DateTime(2026, 10, 12));

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            PeriodStart = new DateTime(2026, 10, 1),
            PeriodEnd = new DateTime(2026, 10, 10, 8, 30, 0)
        });

        Assert.Equal(new[] { no }, nos);
    }

    [Fact]
    public async Task Index_區間開始帶有時間時仍以日期為準_當天算有交集()
    {
        var no = await AddFilterTripAsync(new DateTime(2026, 10, 10), new DateTime(2026, 10, 12));

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            PeriodStart = new DateTime(2026, 10, 12, 18, 0, 0),
            PeriodEnd = new DateTime(2026, 10, 20)
        });

        Assert.Equal(new[] { no }, nos);
    }

    [Fact]
    public async Task Index_區間結束早於開始時被擋下並說明原因()
    {
        var result = await CreateService().Index(new BusinessTripSearchVM
        {
            PeriodStart = new DateTime(2026, 10, 12),
            PeriodEnd = new DateTime(2026, 10, 11)
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.FieldValueIsInvalid, result.ErrorCode);
        Assert.Contains("不得早於", result.ErrorMessage);
    }

    [Fact]
    public async Task Index_日期區間與其他條件可組合()
    {
        var target = await AddFilterTripAsync(
            new DateTime(2026, 10, 10), new DateTime(2026, 10, 12), BusinessTripStatus.已核准, travelerStaffId: _otherStaffId);
        await AddFilterTripAsync(
            new DateTime(2026, 10, 10), new DateTime(2026, 10, 12), BusinessTripStatus.草稿, travelerStaffId: _otherStaffId);
        await AddFilterTripAsync(
            new DateTime(2026, 12, 1), new DateTime(2026, 12, 2), BusinessTripStatus.已核准, travelerStaffId: _otherStaffId);

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            TravelerStaffId = _otherStaffId,
            Status = (int)BusinessTripStatus.已核准,
            PeriodStart = new DateTime(2026, 10, 1),
            PeriodEnd = new DateTime(2026, 10, 31)
        });

        Assert.Equal(new[] { target }, nos);
    }

    // ---------- 出差單管理權限 ----------

    private const int AdminUserId = 30;
    private const int AdminRoleId = 2;
    private const int PlainRoleId = 3;

    /// <summary>角色 2 有「出差單管理」，角色 3 只有「出差單」。</summary>
    private void SeedRolePermissions()
    {
        using var db = _testDb.CreateContext();
        db.Role.Add(new Role { Id = AdminRoleId, RoleName = "出差單管理員" });
        db.Role.Add(new Role { Id = PlainRoleId, RoleName = "一般出差單使用者" });
        db.Permission.Add(new Permission { RoleId = AdminRoleId, PageId = (int)PermissionType.出差單管理 });
        db.Permission.Add(new Permission { RoleId = PlainRoleId, PageId = (int)PermissionType.出差單 });
        db.SaveChanges();
    }

    /// <summary>與目前使用者、管理者都無關的出差單（OtherUserId 建立、出差人是同事）。</summary>
    private Task<string> AddUnrelatedTripAsync(BusinessTripStatus status = BusinessTripStatus.草稿)
    {
        return AddFilterTripAsync(
            new DateTime(2026, 10, 10), new DateTime(2026, 10, 12), status, userId: OtherUserId, travelerStaffId: _otherStaffId);
    }

    [Fact]
    public async Task Index_有出差單管理權限者看得到全部出差單()
    {
        SeedRolePermissions();
        var mine = await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1));
        var unrelated = await AddUnrelatedTripAsync();

        var nos = await IndexNosAsync(new BusinessTripSearchVM(), AdminUserId, AdminRoleId);

        Assert.Equal(new[] { mine, unrelated }.OrderBy(x => x), nos);
    }

    [Fact]
    public async Task Index_只有出差單權限的一般使用者看不到與自己無關的單()
    {
        SeedRolePermissions();
        var unrelated = await AddUnrelatedTripAsync();

        var nos = await IndexNosAsync(new BusinessTripSearchVM(), CurrentUserId, PlainRoleId);

        Assert.DoesNotContain(unrelated, nos);
        Assert.Empty(nos);
    }

    [Fact]
    public async Task Index_管理權限依角色判斷_同一個使用者換成沒有管理權限的角色就看不到()
    {
        SeedRolePermissions();
        var unrelated = await AddUnrelatedTripAsync();

        var asAdmin = await IndexNosAsync(new BusinessTripSearchVM(), CurrentUserId, AdminRoleId);
        var asPlain = await IndexNosAsync(new BusinessTripSearchVM(), CurrentUserId, PlainRoleId);

        Assert.Equal(new[] { unrelated }, asAdmin);
        Assert.Empty(asPlain);
    }

    [Fact]
    public async Task Index_管理權限者也能用篩選條件縮小結果()
    {
        SeedRolePermissions();
        await AddFilterTripAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 1), BusinessTripStatus.草稿);
        var target = await AddUnrelatedTripAsync(BusinessTripStatus.已核准);
        await AddUnrelatedTripAsync(BusinessTripStatus.草稿);

        var nos = await IndexNosAsync(new BusinessTripSearchVM
        {
            Status = (int)BusinessTripStatus.已核准,
            TravelerStaffId = _otherStaffId,
            PeriodStart = new DateTime(2026, 10, 12),
            PeriodEnd = new DateTime(2026, 10, 20)
        }, AdminUserId, AdminRoleId);

        Assert.Equal(new[] { target }, nos);
    }

    [Fact]
    public async Task Index_管理權限者的待我簽核篩選仍只列出輪到自己簽的單()
    {
        SeedRolePermissions();
        var mine = await AddUnrelatedTripAsync(BusinessTripStatus.簽核中);
        await AddUnrelatedTripAsync(BusinessTripStatus.簽核中);
        _approval.PendingNotify.Add(new GetApprovalNotifyVM { TableType = (int)TableType.出差單, TableId = mine });

        var nos = await IndexNosAsync(new BusinessTripSearchVM { PendingMyApproval = true }, AdminUserId, AdminRoleId);

        Assert.Equal(new[] { mine }, nos);
    }

    [Fact]
    public async Task Index_管理權限者看到他人的單時不具備任何操作權()
    {
        SeedRolePermissions();
        await AddUnrelatedTripAsync(BusinessTripStatus.草稿);
        await AddUnrelatedTripAsync(BusinessTripStatus.簽核中);
        await AddUnrelatedTripAsync(BusinessTripStatus.已退回);
        await AddUnrelatedTripAsync(BusinessTripStatus.已核准);

        var result = await CreateService(AdminUserId, AdminRoleId).Index(new BusinessTripSearchVM());

        Assert.Equal(4, result.Data!.TotalCount);
        Assert.All(result.Data.Items, x =>
        {
            Assert.False(x.CanEdit);
            Assert.False(x.CanApprove);
            Assert.False(x.CanRevoke);
            Assert.False(x.CanVoid);
        });
    }

    [Fact]
    public async Task Index_管理權限者分頁總筆數涵蓋全部()
    {
        SeedRolePermissions();
        for (var i = 0; i < 3; i++)
        {
            await AddUnrelatedTripAsync();
        }

        var result = await CreateService(AdminUserId, AdminRoleId).Index(new BusinessTripSearchVM { PageSize = 2 });

        Assert.Equal(3, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);
    }

    [Fact]
    public async Task Get_有出差單管理權限者可檢視與自己無關的出差單_一般使用者仍看不到()
    {
        SeedRolePermissions();
        var no = await AddUnrelatedTripAsync();
        var id = LoadTrip(no).Id;

        var asAdmin = await CreateService(AdminUserId, AdminRoleId).Get(id);
        var asPlain = await CreateService(CurrentUserId, PlainRoleId).Get(id);

        Assert.True(asAdmin.IsSuccess, asAdmin.ErrorMessage);
        Assert.Equal(no, asAdmin.Data!.No);
        Assert.False(asAdmin.Data.CanEdit);
        Assert.False(asPlain.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, asPlain.ErrorCode);
    }

    [Fact]
    public async Task 管理權限不賦予編輯與刪除_對他人的單視為找不到()
    {
        SeedRolePermissions();
        var no = await AddUnrelatedTripAsync();
        var id = LoadTrip(no).Id;
        var service = CreateService(AdminUserId, AdminRoleId);

        var edit = await service.Edit(new BusinessTripAddVM
        {
            Id = id,
            TravelerStaffId = _otherStaffId,
            Destination = "被改掉",
            Reason = "被改掉",
            StartDate = new DateTime(2026, 10, 10),
            EndDate = new DateTime(2026, 10, 12)
        });
        var delete = await service.Delete(id);

        Assert.Equal(ErrorCodeType.NotFoundData, edit.ErrorCode);
        Assert.Equal(ErrorCodeType.NotFoundData, delete.ErrorCode);
        Assert.Equal("台中客戶端", LoadTrip(no).Destination);
    }

    [Fact]
    public async Task 管理權限不賦予送簽_撤銷_核准_拒絕_作廢_對他人的單視為找不到()
    {
        SeedRolePermissions();
        var draft = await AddUnrelatedTripAsync(BusinessTripStatus.草稿);
        var inApproval = await AddUnrelatedTripAsync(BusinessTripStatus.簽核中);
        var approved = await AddUnrelatedTripAsync(BusinessTripStatus.已核准);
        var service = CreateService(AdminUserId, AdminRoleId);

        var results = new[]
        {
            await service.SendApproval(Vm(draft)),
            await service.RevokeApproval(Vm(inApproval)),
            await service.Approval(Vm(inApproval)),
            await service.RejectApproval(new ApprovalVM { TableId = inApproval, Memo = "不行" }),
            await service.VoidApproval(Vm(approved))
        };

        Assert.All(results, r =>
        {
            Assert.False(r.IsSuccess);
            Assert.Equal(ErrorCodeType.NotFoundData, r.ErrorCode);
        });
        Assert.Equal((int)BusinessTripStatus.草稿, LoadTrip(draft).Status);
        Assert.Equal((int)BusinessTripStatus.簽核中, LoadTrip(inApproval).Status);
        Assert.Equal((int)BusinessTripStatus.已核准, LoadTrip(approved).Status);
    }
}
