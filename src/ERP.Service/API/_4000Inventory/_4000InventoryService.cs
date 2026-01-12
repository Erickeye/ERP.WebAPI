using System.Linq.Expressions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.QuerySort;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4000InventoryService
    {
        Task<ResultModel<PagedResult<InventoryVM>>> Index(InventorySearchVM vm);
    }
    public class _4000InventoryService : I_4000InventoryService
    {
        private readonly ERPDbContext _db;

        public _4000InventoryService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PagedResult<InventoryVM>>> Index(InventorySearchVM vm)
        {
            var filter = SearchExpressionBuilder.Build<t_4000Inventory>(vm);

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

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<InventoryVM>());
            }
            var pagedResult = await query.ToPagedResultAsync(vm);

            return ResultModel.Ok(pagedResult);
        }
    }
}
