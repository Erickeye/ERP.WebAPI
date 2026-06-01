using ERP.Approval.Abstractions;
using ERP.Library.Enums.Other;

namespace ERP.Approval.Services;

public class ApprovalTargetHandlerResolver : IApprovalTargetHandlerResolver
{
    private readonly IReadOnlyDictionary<TableType, IApprovalTargetHandler> _handlers;

    public ApprovalTargetHandlerResolver(IEnumerable<IApprovalTargetHandler> handlers)
    {
        _handlers = handlers
            .GroupBy(x => x.TableType)
            .ToDictionary(x => x.Key, x => x.First());
    }

    public IApprovalTargetHandler? Resolve(TableType tableType)
    {
        return _handlers.TryGetValue(tableType, out var handler) ? handler : null;
    }
}
