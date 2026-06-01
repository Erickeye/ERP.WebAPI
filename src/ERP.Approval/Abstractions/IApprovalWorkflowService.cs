using ERP.Library.ViewModels;

namespace ERP.Approval.Abstractions;

public interface IApprovalWorkflowService
{
    Task<ResultModel<string>> SendApprovalProcess(ApprovalVM data);
    Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(ApprovalVM data);
    Task<ResultModel<string>> Approval(ApprovalVM data);
    Task<ResultModel<string>> RejectApproval(ApprovalVM data);
    Task<ResultModel<ListResult<GetApprovalNotifyVM>>> GetApprovalNotify();
    Task<ResultModel<string>> RevokeApproval(ApprovalVM data);
    Task<ResultModel<string>> VoidApproval(ApprovalVM data);
}
