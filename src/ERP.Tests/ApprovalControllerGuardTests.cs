using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Tests.Support;
using ERP.WebAPI.Controllers.Approval;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ERP.Tests;

/// <summary>
/// 通用簽核端點不可繞過領料單與出差單的專用端點（ADR 0001、差勤 ADR 0001）。
/// </summary>
public class ApprovalControllerGuardTests
{
    private static ApprovalController CreateController()
    {
        // 領料單被擋下時不會碰到引擎，因此兩個服務傳 null 即可
        return new ApprovalController(null!, null!);
    }

    private static ApprovalVM MaterialIssueVm() => new ApprovalVM { TableType = TableType.領料單, TableId = "IS1151001" };

    private static ApprovalVM BusinessTripVm() => new ApprovalVM { TableType = TableType.出差單, TableId = "BT1151001" };

    private static void AssertBlocked(IActionResult actionResult)
    {
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<ResultModel<string>>(ok.Value);
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidApproval, result.ErrorCode);
    }

    [Fact]
    public async Task 送出簽核_領料單被擋() => AssertBlocked(await CreateController().SendApprovalProcess(MaterialIssueVm()));

    [Fact]
    public async Task 簽核_領料單被擋() => AssertBlocked(await CreateController().Approval(MaterialIssueVm()));

    [Fact]
    public async Task 拒絕簽核_領料單被擋() => AssertBlocked(await CreateController().RejectApproval(MaterialIssueVm()));

    [Fact]
    public async Task 撤銷簽核_領料單被擋() => AssertBlocked(await CreateController().RevokeApproval(MaterialIssueVm()));

    [Fact]
    public async Task 作廢簽核_領料單被擋() => AssertBlocked(await CreateController().VoidApproval(MaterialIssueVm()));

    [Fact]
    public async Task 送出簽核_出差單被擋() => AssertBlocked(await CreateController().SendApprovalProcess(BusinessTripVm()));

    [Fact]
    public async Task 簽核_出差單被擋() => AssertBlocked(await CreateController().Approval(BusinessTripVm()));

    [Fact]
    public async Task 拒絕簽核_出差單被擋() => AssertBlocked(await CreateController().RejectApproval(BusinessTripVm()));

    [Fact]
    public async Task 撤銷簽核_出差單被擋() => AssertBlocked(await CreateController().RevokeApproval(BusinessTripVm()));

    [Fact]
    public async Task 作廢簽核_出差單被擋() => AssertBlocked(await CreateController().VoidApproval(BusinessTripVm()));
}
