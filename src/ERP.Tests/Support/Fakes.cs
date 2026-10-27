using ERP.Approval.Abstractions;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Service.API;
using ERP.Service.API.AMS;
using ERP.Service.Sftp;
using Microsoft.AspNetCore.Http;

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
/// 假的 SFTP 服務：檔案存在記憶體，不實際連線；可設定上傳或下載失敗。
/// </summary>
public class FakeSftpService : ISftpService
{
    public Dictionary<string, byte[]> Files { get; } = new();

    public bool FailUpload { get; set; }

    public ResultModel<string> UploadFile(IFormFile file) => throw new NotSupportedException();

    public ResultModel<string> UploadStream(Stream content, string remotePath)
    {
        if (FailUpload)
        {
            return ResultModel.Error(ErrorCodeType.Exception, "上傳失敗：模擬連線錯誤");
        }

        using var buffer = new MemoryStream();
        content.CopyTo(buffer);
        Files[remotePath] = buffer.ToArray();
        return ResultModel.Ok(remotePath);
    }

    public ResultModel<Stream?> DownloadFile(string remotePath)
    {
        if (!Files.TryGetValue(remotePath, out var bytes))
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData);
        }

        return ResultModel.Ok((Stream?)new MemoryStream(bytes))!;
    }

    public ResultModel<string> DeleteFile(string remotePath)
    {
        return Files.Remove(remotePath)
            ? ResultModel.Ok("檔案已成功刪除")
            : ResultModel.Error(ErrorCodeType.NotFoundData);
    }

    public ResultModel<ListResult<string>> ListFiles() => ResultModel.Ok(Files.Keys.ToList());
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

    public Func<Task>? OnReject { get; set; }

    public Func<Task>? OnSend { get; set; }

    public Func<Task>? OnRevoke { get; set; }

    public Func<Task>? OnVoid { get; set; }

    /// <summary>引擎「待簽核」查詢回傳的項目（目前登入者輪到簽的單據）。</summary>
    public List<GetApprovalNotifyVM> PendingNotify { get; set; } = new();

    public async Task<ResultModel<string>> SendApprovalProcess(ApprovalVM data)
    {
        if (OnSend != null)
        {
            await OnSend();
        }
        return SendResult;
    }

    public async Task<ResultModel<string>> Approval(ApprovalVM data)
    {
        if (OnApproval != null)
        {
            await OnApproval();
        }
        return ApprovalResult;
    }

    public async Task<ResultModel<string>> RejectApproval(ApprovalVM data)
    {
        if (OnReject != null)
        {
            await OnReject();
        }
        return RejectResult;
    }

    public async Task<ResultModel<string>> RevokeApproval(ApprovalVM data)
    {
        if (OnRevoke != null)
        {
            await OnRevoke();
        }
        return RevokeResult;
    }

    public async Task<ResultModel<string>> VoidApproval(ApprovalVM data)
    {
        if (OnVoid != null)
        {
            await OnVoid();
        }
        return VoidResult;
    }

    public Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(ApprovalVM data)
        => throw new NotSupportedException();

    public Task<ResultModel<ListResult<GetApprovalNotifyVM>>> GetApprovalNotify()
        => Task.FromResult(ResultModel.Ok(PendingNotify.ToList()));
}
