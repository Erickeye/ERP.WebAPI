using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;
using Aspose.Cells;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4000InventoryService
    {
        Task<ResultModel<PagedResult<InventoryVM>>> Index(InventorySearchVM vm);
        Task<ResultModel<FileModel>> Export(InventorySearchVM vm);
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
            var query = QueryList(vm);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<InventoryVM>());
            }
            var pagedResult = await query.ToPagedResultAsync(vm);

            return ResultModel.Ok(pagedResult);
        }
     
        public async Task<ResultModel<FileModel>> Export(InventorySearchVM vm)
        {
            //var query = QueryList(vm);

            var list = await _db.Database.StoredProcedureQueryAsync<InventoryVM>(
                "sp_Inventory_Search",
                new
                {
                    vm.SupplierName,
                    vm.LocationName,
                    vm.Category,
                    vm.Name,
                });

            var reportData = new InventoryExportVM
            {
                Items = list.Select(x => new InventoryExportItemVM
                {
                    SupplierName = x.SupplierName,
                    Name = x.Name,
                    LocationName = x.LocationName,
                    Category = x.Category,
                    LastPurchaseDate = x.LastPurchaseDate?.ToString("yyyy/MM/dd"),
                    No = x.No,
                    Unit = x.Unit,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Total = x.Total,
                }).ToList()
            };

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "庫存列表.xlsx");

            var workbook = new Workbook(templatePath);
            workbook.BindData(reportData);

            var file = workbook.ConvertToFileModel((Aspose.Cells.SaveFormat)vm.SaveFormat, "退費資料列表");

            return ResultModel.Ok(file);
        }
        public IQueryable<InventoryVM> QueryList(InventorySearchVM vm)
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
                    No = x.No,
                    Unit = x.Unit,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Total = x.Total
                });

            return query;
        }
    }
}
