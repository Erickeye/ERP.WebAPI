using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using System.Linq.Expressions;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;
using ERP.Library.Extensions;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4000InventoryService
    {
        Task<ResultModel<PagedResult<InventoryVM>>> Index(InventorySearchVM vm);
    }
    public class _4000InventoryController : I_4000InventoryService
    {
        private readonly ERPDbContext _db;

        public _4000InventoryController(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PagedResult<InventoryVM>>> Index(InventorySearchVM vm)
        {
            Expression<Func<t_4000Inventory, bool>> filter = i => true;
            if (!string.IsNullOrEmpty(vm.SupplierName))
            {
                filter = filter.ExpressionAnd(x => x.Supplier != null && x.Supplier.Name!.Contains(vm.SupplierName));
            }

            var query = _db.t_4000Inventory
                .AsNoTracking()
                .Include(i => i.Supplier)
                .Include(i => i.Location)
                .Where(filter)
                .Select(x => new InventoryVM
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    SupplierName = x.Supplier != null ? x.Supplier.Name : string.Empty,
                    Name = x.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location != null ? x.Location.Name : string.Empty,
                    Category = x.Category,
                    LastPurchaseDate = x.LastPurchaseDate,
                    Number = x.Number,
                    Unit = x.Unit,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Total = x.Total
                });

            var pagedResult = await query.ToPagedResultAsync(vm);

            return ResultModel.Ok(pagedResult);
        }
    }
}
