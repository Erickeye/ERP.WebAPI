using ERP.Library.Enums.Other;

namespace ERP.Approval.Abstractions;

public interface IApprovalTargetHandlerResolver
{
    IApprovalTargetHandler? Resolve(TableType tableType);
}
