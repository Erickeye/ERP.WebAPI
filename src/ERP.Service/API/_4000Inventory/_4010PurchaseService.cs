using System.Linq.Expressions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4010PurchaseService
    {
        Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm);
        Task<ResultModel<PurchaseVM>> Get(int id);
        Task<ResultModel<string>> Add(PurchaseAddVM vm);
        Task<ResultModel<string>> Edit(PurchaseAddVM vm);
    }
    public class _4010PurchaseService : I_4010PurchaseService
    {
        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISerialService _serialService;
        public _4010PurchaseService(ERPDbContext db, ICurrentUserService currentUserService, ISerialService serialService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
        }

        public async Task<ResultModel<PagedResult<PurchaseVM>>> Index(PurchaseSearchVM vm)
        {
            var filter = SearchExpressionBuilder.Build<t_4010Purchase>(vm);

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
                    Items = x.t_4011PurchaseDetail.Select(d => new PurchaseItemVM
                    {
                        Id = d.Id,
                        Category = d.Category,
                        No = d.No,
                        Name = d.Name,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        Total = d.Quantity * d.Price,
                    }).ToList(),
                });

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<PurchaseVM>());
            }
            var PageResult = await query.ToPagedResultAsync(vm);

            return ResultModel.Ok(PageResult);
        }
        public async Task<ResultModel<PurchaseVM>> Get(int id)
        {
            var data = await _db.t_4010Purchase
                .AsNoTracking()
                .Where(x => x.Id == id)
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
                    Items = x.t_4011PurchaseDetail.Select(d => new PurchaseItemVM
                    {
                        Category = d.Category,
                        No = d.No,
                        Name = d.Name,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        Total = d.Quantity * d.Price,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            return ResultModel.Ok(data);
        }
        public async Task<ResultModel<string>> Add(PurchaseAddVM vm)
        {
            var entity = new t_4010Purchase
            {
                CreateTime = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
            };
            await Modify(vm, entity);

            //var purchase = new t_4010Purchase
            //{
            //    No = "",
            //    SupplierId = vm.SupplierId,
            //    CustomerId = vm.CustomerId,
            //    LocationId = vm.LocationId,
            //    ProjectName = vm.ProjectName ?? "",
            //    PurchaseDate = vm.PurchaseDate,
            //    IsPurchase = vm.IsPurchase,
            //    PaymentMethodId = vm.PaymentMethodId,
            //    Payer = vm.Payer,
            //    InvoiceNumber = vm.InvoiceNumber,
            //    Note = vm.Note,
            //    CreateTime = DateTime.Now,
            //    CreateUserId = _currentUserService.UserId,
            //};
            //var purchaseItems = vm.Items.Select(x => new t_4011PurchaseDetail
            //{
            //    Category = x.Category,
            //    No = x.No,
            //    Name = x.Name ?? "",
            //    Unit = x.Unit,
            //    Quantity = x.Quantity,
            //    Price = x.Price,
            //    Total = x.Quantity * x.Price,
            //}).ToList();

            //purchase.t_4011PurchaseDetail = purchaseItems;

            //_db.t_4010Purchase.Add(purchase);

            entity.No = await _serialService.GenerateAsync("PU");
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.No);
        }
        public async Task<ResultModel<string>> Edit(PurchaseAddVM vm)
        {
            var entity = await _db.t_4010Purchase
                .Include(x => x.t_4011PurchaseDetail)
                .Where(x => x.Id == vm.Id)
                .FirstOrDefaultAsync();

            if(entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            await Modify(vm, entity);

            await _db.SaveChangesAsync();
            return ResultModel.Ok(entity.No!);
        }
        private async Task<ResultModel<string>> Modify(PurchaseAddVM vm, t_4010Purchase entity)
        {
            entity.SupplierId = vm.SupplierId;
            entity.CustomerId = vm.CustomerId;
            entity.LocationId = vm.LocationId;
            entity.ProjectName = vm.ProjectName ?? "";
            entity.PurchaseDate = vm.PurchaseDate;
            entity.IsPurchase = vm.IsPurchase;
            entity.PaymentMethodId = vm.PaymentMethodId;
            entity.Payer = vm.Payer;
            entity.InvoiceNumber = vm.InvoiceNumber;
            entity.Note = vm.Note;

            //---處理細項---

            //刪除細項
            var deleteItems = entity.t_4011PurchaseDetail
                .Where(x => !vm.Items.Any(c => c.Id == x.Id))
                .ToList();

            _db.t_4011PurchaseDetail.RemoveRange(deleteItems);

            foreach (var item in vm.Items)
            {
                var entityItem = entity.t_4011PurchaseDetail
                    .Where(x => x.Id == item.Id)
                    .FirstOrDefault();

                if(entityItem == null)
                {
                    entityItem = new t_4011PurchaseDetail();
                    entity.t_4011PurchaseDetail.Add(entityItem);
                }
                entityItem.Category = item.Category;
                entityItem.No = item.No;
                entityItem.Name = item.Name ?? "";
                entityItem.Unit = item.Unit;
                entityItem.Quantity = item.Quantity;
                entityItem.Price = item.Price;
                entityItem.Total = item.Quantity * item.Price;
            }

            return ResultModel.Ok();
        }
    }
}
