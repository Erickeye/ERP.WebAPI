namespace ERP.Approval.Abstractions;

public interface IApprovalUserContext
{
    int UserId { get; }
    int RoleId { get; }
}
