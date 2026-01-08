using System.Linq.Expressions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4010PurchaseService
    {

    }
    public class _4010PurchaseService : I_4010PurchaseService
    {
        private readonly ERPDbContext _db;
        public _4010PurchaseService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm)
        {
            Expression<Func<t_4010Purchase,bool>> filter = x => true;
            if (!string.IsNullOrWhiteSpace(vm.No))
            {
                filter = filter.ExpressionAnd(x => x.No!.Contains(vm.No));
            }
            if (!string.IsNullOrWhiteSpace(vm.SupplierName))
            {
                filter = filter.ExpressionAnd(x => x.Supplier!.Name!.Contains(vm.SupplierName));
            }
            if(!string.IsNullOrWhiteSpace(vm.CustomerName))
            {
                filter = filter.ExpressionAnd(x => x.Customer!.Name!.Contains(vm.CustomerName));
            }
            if (!string.IsNullOrWhiteSpace(vm.ProjectName))
            {
                filter = filter.ExpressionAnd(x => x.ProjectName!.Contains(vm.ProjectName));
            }

            var query = _db.t_4010Purchase
                .AsNoTracking()
                .Where(filter)
                .Select(x => new PurchaseVM
                {
                    Id = x.Id,
                    No = x.No,
                    SupplierId = x.SupplierId,
                    SupplierName = x.Supplier!.Name,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer!.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location!.Name,
                    ProjectName = x.ProjectName,
                    PurchaseDate = x.PurchaseDate,
                    IsPurchase = x.IsPurchase,
                    PaymentMethodId = x.PaymentMethodId,
                    PaymentMethodName = x.PaymentMethod!.Name,
                    Amount = x.Amount,
                    Tax = x.Tax,
                    Payer = x.Payer,
                    InvoiceNumber = x.InvoiceNumber,
                    Note = x.Note,
                    Authorizator = x.Authorizator,
                    IsApproval = x.IsApproval,
                    CreateTime = x.CreateTime,
                    CreateUserId = x.CreateUserId
                });

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<PurchaseVM>());
            }
            var PageResult = await query.ToPagedResultAsync(vm);

            return ResultModel.Ok(PageResult);
        }


    }
}
