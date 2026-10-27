using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.Library.Enums.Other;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.Approval
{
    public class ContractApprovalTargetHandler : IApprovalTargetHandler
    {
        private readonly ERPDbContext _context;

        public ContractApprovalTargetHandler(ERPDbContext context)
        {
            _context = context;
        }

        public TableType TableType => TableType.合約單;

        public Task<bool> ExistsAsync(string tableId)
        {
            return _context.t_3050Contract.AnyAsync(x => x.No == tableId);
        }
    }
}
