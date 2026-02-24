using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
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
        Task<ResultModel<string>> SendApproval(ApprovalVM vm);
        Task<ResultModel<string>> Approval(ApprovalVM vm);
        Task<ResultModel<string>> RevokeApproval(ApprovalVM vm);
    }
    public class _4010PurchaseService : I_4010PurchaseService
    {
        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISerialService _serialService;
        private readonly IApprovalService _approvalService;
        public _4010PurchaseService(ERPDbContext db, ICurrentUserService currentUserService, ISerialService serialService, IApprovalService approvalService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
            _approvalService = approvalService;
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
            var result = Modify(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

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

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            var result = Modify(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            await _db.SaveChangesAsync();
            return ResultModel.Ok(entity.No!);
        }
        private ResultModel<string> Modify(PurchaseAddVM vm, t_4010Purchase entity)
        {
            entity.SupplierId = vm.SupplierId;
            entity.CustomerId = vm.CustomerId;
            entity.LocationId = vm.LocationId;
            entity.ProjectName = vm.ProjectName ?? "";
            entity.PurchaseDate = vm.PurchaseDate;
            entity.IsPurchase = vm.IsPurchase;
            entity.PaymentMethodId = vm.PaymentMethodId;
            entity.Payer = vm.Payer ?? "";
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

                if (entityItem == null)
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
        public async Task<ResultModel<string>> SendApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.進貨單;
            var result = await _approvalService.SendApprovalProcess(vm);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            await _db.SaveChangesAsync();
            return ResultModel.Ok($"{result.Data}");
        }
        public async Task<ResultModel<string>> Approval(ApprovalVM vm)
        {
            vm.TableType = TableType.進貨單;
            var result = await _approvalService.Approval(vm);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }
            if(!result.Data!.Contains("全數完成"))
            {
                return ResultModel.Ok($"{result.Data}");
            }

            //簽核全數完成後 => 更新庫存數量
            var purchase = await _db.t_4010Purchase
                .Include(x => x.t_4011PurchaseDetail)
                .Where(x => x.No == vm.TableId)
                .FirstOrDefaultAsync();

            var purchaseItem = purchase!.t_4011PurchaseDetail.ToList();
            var purchaseItemNo = purchaseItem.Select(x => x.No).ToList();

            //抓出庫存
            var inventories = await _db.t_4000Inventory
                .Where(x =>
                    x.LocationId == purchase.LocationId &&
                    x.SupplierId == purchase.SupplierId &&
                    purchaseItemNo.Contains(x.Number)
                )
                .ToListAsync();

            var now = DateTime.Now;

            foreach (var item in purchaseItem)
            {
                var inventory = inventories
                     .Where(x => x.Number == item.No)
                     .FirstOrDefault();

                if (inventory != null)
                {
                    inventory.Quantity += (decimal)item.Quantity!;
                }
                else
                {
                    //庫存沒有則新增一筆
                    inventory = new t_4000Inventory
                    {
                        SupplierId = purchase.SupplierId,
                        Name = item.Name,
                        LocationId = purchase.LocationId,
                        Category = item.Category,
                        Number = item.No,
                        Unit = item.Unit,
                        Quantity = item.Quantity,
                        Amount = item.Price,
                        Total = item.Quantity * item.Price,
                        LastPurchaseDate = now
                    };
                    _db.t_4000Inventory.Add(inventory);
                }
            }
            purchase.IsApproval = true;

            await _db.SaveChangesAsync();
            return ResultModel.Ok($"{result.Data}");
        }
        public async Task<ResultModel<string>> RevokeApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.進貨單;
            var result = await _approvalService.RevokeApproval(vm);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }
            //撤銷簽核成功後 => 更新庫存數量
            var purchase = await _db.t_4010Purchase
                .Include(x => x.t_4011PurchaseDetail)
                .Where(x => x.No == vm.TableId)
                .FirstOrDefaultAsync();

            var purchaseItem = purchase!.t_4011PurchaseDetail.ToList();
            var purchaseItemNo = purchaseItem.Select(x => x.No).ToList();

            //抓出庫存
            var inventories = await _db.t_4000Inventory
                .Where(x =>
                    x.LocationId == purchase.LocationId &&
                    x.SupplierId == purchase.SupplierId &&
                    purchaseItemNo.Contains(x.Number)
                )
                .ToListAsync();

            foreach (var item in purchase!.t_4011PurchaseDetail)
            {
                var inventory = inventories
                     .Where(x => x.Number == item.No)
                     .FirstOrDefault();

                if (inventory == null)
                {
                    return ResultModel.Error(ErrorCodeType.InvalidApproval, "撤銷簽核失敗，該庫存數量不足。");
                }
                inventory.Quantity -= (decimal)item.Quantity!;
            }
            await _db.SaveChangesAsync();
            return ResultModel.Ok($"{result.Data}");
        }

    }
}
