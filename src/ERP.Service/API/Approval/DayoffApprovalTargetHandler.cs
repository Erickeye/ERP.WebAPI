using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.Library.Enums.Other;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.Approval;

public class DayoffApprovalTargetHandler : IApprovalTargetHandler
{
    private readonly ERPDbContext _context;

    public DayoffApprovalTargetHandler(ERPDbContext context)
    {
        _context = context;
    }

    public TableType TableType => (TableType)1;

    public Task<bool> ExistsAsync(string tableId)
    {
        return _context.t_1030Dayoff.AnyAsync(x => x.Id.ToString() == tableId);
    }
}
