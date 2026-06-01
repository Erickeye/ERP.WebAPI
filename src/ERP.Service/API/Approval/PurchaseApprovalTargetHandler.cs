using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.Library.Enums.Other;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.Approval;

public class PurchaseApprovalTargetHandler : IApprovalTargetHandler
{
    private readonly ERPDbContext _context;

    public PurchaseApprovalTargetHandler(ERPDbContext context)
    {
        _context = context;
    }

    public TableType TableType => (TableType)7;

    public Task<bool> ExistsAsync(string tableId)
    {
        return _context.t_4010Purchase.AnyAsync(x => x.No!.ToString() == tableId);
    }
}
