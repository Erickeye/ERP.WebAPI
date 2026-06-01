using ERP.Approval.Abstractions;
using ERP.Service.API.AMS;

namespace ERP.Service.API.Approval;

public class ApprovalCurrentUserContext : IApprovalUserContext
{
    private readonly ICurrentUserService _currentUserService;

    public ApprovalCurrentUserContext(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public int UserId => _currentUserService.UserId;

    public int RoleId => _currentUserService.RoleId;
}
