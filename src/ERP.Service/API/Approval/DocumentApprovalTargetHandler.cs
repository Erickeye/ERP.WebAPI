using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.Library.Enums.Other;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.Approval;

public class DocumentApprovalTargetHandler : IApprovalTargetHandler
{
    private readonly ERPDbContext _context;

    public DocumentApprovalTargetHandler(ERPDbContext context)
    {
        _context = context;
    }

    public TableType TableType => (TableType)3;

    public Task<bool> ExistsAsync(string tableId)
    {
        return _context.t_1040Document.AnyAsync(x => x.Id.ToString() == tableId);
    }
}
