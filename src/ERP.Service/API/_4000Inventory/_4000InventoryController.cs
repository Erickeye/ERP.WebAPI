using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using System.Linq.Expressions;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;
using ERP.Library.Extensions;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4000InventoryService
    {
    }
    public class _4000InventoryController : I_4000InventoryService
    {
        private readonly ERPDbContext _db;

        public _4000InventoryController(ERPDbContext db)
        {
            _db = db;
        }

        //public async Task<ResultModel<InventoryVM>> Index(InventorySearchVM vm)
        //{
        //    Expression<Func<t_4000Inventory, bool>> filter = i => true;
        //    if (!string.IsNullOrEmpty(vm.SupplierName))
        //    {
        //        filter = filter.ExpressionAnd(x => x.SupplierId.Name.Contains(vm.SupplierName));
        //    }

        //    var inventory = await (
        //        from i in _db.t_4000Inventory
        //        join s in _db.t_4060Supplier on i.SupplierId equals s.Id into supplierJoin
        //        from s in supplierJoin.DefaultIfEmpty() // 左外連接
        //        join l in _db.SystemConfig on i.LocationId equals l.Id into locationJoin
        //        from l in locationJoin.DefaultIfEmpty() // 左外連接
        //        select new InventoryVM
        //        {
        //            Id = i.Id,
        //            SupplierId = i.SupplierId,
        //            SupplierName = s.Name,
        //            Name = i.Name,
        //            LocationId = i.LocationId,
        //            LocationName = l != null ? l.Name : null,
        //            Category = i.Category,
        //            LastPurchaseDate = i.LastPurchaseDate,
        //            Number = i.Number,
        //            Unit = i.Unit,
        //            Quantity = i.Quantity,
        //            Amount = i.Amount,
        //            Total = i.Total
        //        }
        //    ).ToListAsync();
        //}
    }
}
