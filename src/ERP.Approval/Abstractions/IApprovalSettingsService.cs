using ERP.EntityModels.Models;
using ERP.Library.ViewModels;

namespace ERP.Approval.Abstractions;

public interface IApprovalSettingsService
{
    Task<ResultModel<ListResult<ApprovalSettings>>> SettingsIndex();
    Task<ResultModel<ApprovalCheckSettingsVM>> CheckSettings(int approvalSettingsId);
    Task<ResultModel<string>> CreateSettings(ApprovalSettingsInputVM vm);
    Task<ResultModel<string>> EditSetting(ApprovalCheckSettingsVM vm);
    Task<ResultModel<string>> DeleteSettings(int id);
}
