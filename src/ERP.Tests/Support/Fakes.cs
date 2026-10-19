using ERP.Approval.Abstractions;
using ERP.Library.ViewModels;
using ERP.Service.API;
using ERP.Service.API.AMS;

namespace ERP.Tests.Support;

public class FakeCurrentUser : ICurrentUserService
{
    public int UserId { get; set; } = 1;
    public string? UserName { get; set; } = "tester";
    public int RoleId { get; set; } = 1;
    public string? Role { get; set; } = "Admin";
}

public class FakeSerialService : ISerialService
{
    private int _next;

    public Task<string> GenerateAsync(string prefix)
    {
        _next++;
        return Task.FromResult($"{prefix}1151{_next:000}");
    }
}

/// <summary>
/// 假的簽核引擎：回傳可設定的結果，並可在呼叫時執行動作
/// （例如模擬引擎自己寫入一筆紀錄，用來驗證失敗時整筆回滾）。
/// </summary>
public class FakeApprovalWorkflowService : IApprovalWorkflowService
{
    public ResultModel<string> SendResult { get; set; } = ResultModel<string>.Ok("已送出");
    public ResultModel<string> ApprovalResult { get; set; } = ResultModel<string>.Ok("簽核完成，全部流程已結束");
    public ResultModel<string> RejectResult { get; set; } = ResultModel<string>.Ok("已退回");
    public ResultModel<string> RevokeResult { get; set; } = ResultModel<string>.Ok("已撤銷");
    public ResultModel<string> VoidResult { get; set; } = ResultModel<string>.Ok("已作廢");

    public Func<Task>? OnApproval { get; set; }

    public Task<ResultModel<string>> SendApprovalProcess(ApprovalVM data) => Task.FromResult(SendResult);

    public async Task<ResultModel<string>> Approval(ApprovalVM data)
    {
        if (OnApproval != null)
        {
            await OnApproval();
        }
        return ApprovalResult;
    }

    public Task<ResultModel<string>> RejectApproval(ApprovalVM data) => Task.FromResult(RejectResult);
    public Task<ResultModel<string>> RevokeApproval(ApprovalVM data) => Task.FromResult(RevokeResult);
    public Task<ResultModel<string>> VoidApproval(ApprovalVM data) => Task.FromResult(VoidResult);

    public Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(ApprovalVM data)
        => throw new NotSupportedException();

    public Task<ResultModel<ListResult<GetApprovalNotifyVM>>> GetApprovalNotify()
        => throw new NotSupportedException();
}
