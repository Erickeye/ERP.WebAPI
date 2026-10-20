using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.Library.Enums.Other;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.Approval
{
    public class BusinessTripApprovalTargetHandler : IApprovalTargetHandler
    {
        private readonly ERPDbContext _context;

        public BusinessTripApprovalTargetHandler(ERPDbContext context)
        {
            _context = context;
        }

        public TableType TableType => TableType.出差單;

        public Task<bool> ExistsAsync(string tableId)
        {
            return _context.t_1060BusinessTrip.AnyAsync(x => x.No == tableId);
        }
    }
}
