using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;

namespace ERP.Approval.Abstractions;

public interface IApprovalTargetHandler
{
    TableType TableType { get; }

    Task<bool> ExistsAsync(string tableId);

    Task<ResultModel<string>> OnApprovedAsync(ApprovalVM data)
    {
        return Task.FromResult(ResultModel.Ok());
    }

    Task<ResultModel<string>> OnVoidedAsync(ApprovalVM data)
    {
        return Task.FromResult(ResultModel.Ok());
    }
}
