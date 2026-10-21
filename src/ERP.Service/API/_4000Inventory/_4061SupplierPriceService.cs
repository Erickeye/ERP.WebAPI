using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
    /// <summary>
    /// 供應商報價-服務
    /// </summary>
    public interface I_4061SupplierPriceService
    {
        /// <summary>
        /// 查詢某供應商的報價列表（分頁；關鍵字比對料號與品名，預設依料號排序）
        /// </summary>
        Task<ResultModel<PagedResult<SupplierPriceVM>>> Index(SupplierPriceSearchVM vm);

        /// <summary>
        /// 取得單筆供應商報價
        /// </summary>
        Task<ResultModel<SupplierPriceVM>> Get(int id);

        /// <summary>
        /// 新增供應商報價（同一供應商的料號不可重複）
        /// </summary>
        Task<ResultModel<int>> Add(SupplierPriceAddVM vm);

        /// <summary>
        /// 修改供應商報價（可改料號，但不可與同供應商其他報價重複；直接覆蓋，不保留歷史）
        /// </summary>
        Task<ResultModel<string>> Edit(SupplierPriceAddVM vm);

        /// <summary>
        /// 填單查詢：依供應商與料號（去除前後空白後完全相符）取得目前的供應商報價，只回品名、單位、單價。
        /// 查無報價、供應商已停用、供應商不存在都視為成功，Data 為 null（空結果），不是錯誤；供填單參考，不驗證、不阻擋。
        /// </summary>
        Task<ResultModel<SupplierPriceLookupVM?>> Lookup(int supplierId, string? no);

        /// <summary>
        /// 刪除供應商報價（進貨單不引用報價，沒有引用限制）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);
    }

    public class _4061SupplierPriceService : I_4061SupplierPriceService
    {
        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public _4061SupplierPriceService(ERPDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<ResultModel<PagedResult<SupplierPriceVM>>> Index(SupplierPriceSearchVM vm)
        {
            if (!await _db.t_4060Supplier.AnyAsync(x => x.Id == vm.SupplierId))
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到供應商");
            }

            var query = _db.t_4061SupplierPrice
                .AsNoTracking()
                .Where(x => x.SupplierId == vm.SupplierId);

            // 關鍵字同時比對料號與品名
            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var keyword = vm.Keyword.Trim();
                query = query.Where(x => x.No.Contains(keyword) || x.Name.Contains(keyword));
            }

            // 預設依料號排序，最後依建立順序讓分頁穩定
            IQueryable<SupplierPriceVM> list = query
                .OrderBy(x => x.No)
                .ThenBy(x => x.Id)
                .Select(x => new SupplierPriceVM
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    No = x.No,
                    Name = x.Name,
                    Unit = x.Unit,
                    Price = x.Price,
                    Note = x.Note,
                    CreateTime = x.CreateTime,
                    UpdateTime = x.UpdateTime
                });

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                list = list.ApplySort(vm, SortHelper.GetColumns<SupplierPriceVM>());
            }

            var pagedResult = await list.ToPagedResultAsync(vm);
            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<SupplierPriceVM>> Get(int id)
        {
            var data = await _db.t_4061SupplierPrice
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SupplierPriceVM
                {
                    Id = x.Id,
                    SupplierId = x.SupplierId,
                    No = x.No,
                    Name = x.Name,
                    Unit = x.Unit,
                    Price = x.Price,
                    Note = x.Note,
                    CreateTime = x.CreateTime,
                    UpdateTime = x.UpdateTime
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<int>> Add(SupplierPriceAddVM vm)
        {
            var checkResult = await ValidateAsync(vm, ignoreId: null);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            var now = DateTime.Now;
            var userId = _currentUserService.UserId;
            var entity = new t_4061SupplierPrice
            {
                SupplierId = vm.SupplierId,
                CreateTime = now,
                CreateUserId = userId
            };
            Modify(vm, entity, now, userId);

            _db.t_4061SupplierPrice.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.Id);
        }

        public async Task<ResultModel<string>> Edit(SupplierPriceAddVM vm)
        {
            var entity = await _db.t_4061SupplierPrice.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 報價不能換供應商：一律以原報價的供應商為準，輸入的供應商流水號被忽略
            vm.SupplierId = entity.SupplierId;

            var checkResult = await ValidateAsync(vm, ignoreId: entity.Id);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            // 只存現價：直接覆蓋，誰在何時改的由操作紀錄與最後修改欄位追查
            Modify(vm, entity, DateTime.Now, _currentUserService.UserId);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<SupplierPriceLookupVM?>> Lookup(int supplierId, string? no)
        {
            var key = no?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                return ResultModel<SupplierPriceLookupVM?>.Ok(null);
            }

            // 停用或不存在的供應商一律查不到（報價仍保留，重新啟用後即可命中）；料號完全相符，不做模糊比對
            var data = await _db.t_4061SupplierPrice
                .AsNoTracking()
                .Where(x => x.SupplierId == supplierId && x.No == key && x.Supplier.IsActive)
                .Select(x => new SupplierPriceLookupVM
                {
                    Name = x.Name,
                    Unit = x.Unit,
                    Price = x.Price
                })
                .FirstOrDefaultAsync();

            return ResultModel<SupplierPriceLookupVM?>.Ok(data);
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await _db.t_4061SupplierPrice.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 進貨單不引用報價，沒有「被引用」的限制
            _db.t_4061SupplierPrice.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        #region 共用驗證與欄位設定

        /// <summary>
        /// 欄位驗證（必填、長度、單價範圍沿用輸入 VM 的 DataAnnotations，Controller 與 Service 共用同一組規則）加上
        /// 單價小數位數、供應商存在與（供應商＋料號）唯一檢查。會先去除字串前後空白，並把空白字串視為未填。
        /// </summary>
        /// <param name="ignoreId">編輯時排除自己，新增傳 null</param>
        private async Task<ResultModel<string>> ValidateAsync(SupplierPriceAddVM vm, int? ignoreId)
        {
            NormalizeStrings(vm);

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vm, new ValidationContext(vm), validationResults, validateAllProperties: true);
            if (!isValid)
            {
                var messages = validationResults.Select(r =>
                {
                    var member = r.MemberNames.FirstOrDefault();
                    var displayName = member == null
                        ? null
                        : typeof(SupplierPriceAddVM).GetProperty(member)?.GetCustomAttribute<DisplayAttribute>()?.Name;
                    return displayName == null ? r.ErrorMessage : $"{displayName}：{r.ErrorMessage}";
                });
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, string.Join("；", messages));
            }

            // Range 只管大小，小數位數要另外檢查：資料表是 decimal(12, 2)，超過兩位會被默默四捨五入
            if (vm.Price != decimal.Round(vm.Price!.Value, 2))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "單價(新台幣未稅)：最多兩位小數");
            }

            if (!await _db.t_4060Supplier.AnyAsync(x => x.Id == vm.SupplierId))
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到供應商");
            }

            // 唯一索引會擋，但先檢查才能給明確的訊息；品名允許重複，不檢查
            if (await _db.t_4061SupplierPrice.AnyAsync(x => x.SupplierId == vm.SupplierId && x.No == vm.No && x.Id != ignoreId))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "料號：此供應商已有相同料號的報價");
            }

            return ResultModel.Ok();
        }

        private static void NormalizeStrings(SupplierPriceAddVM vm)
        {
            foreach (var property in typeof(SupplierPriceAddVM).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var value = ((string?)property.GetValue(vm))?.Trim();
                property.SetValue(vm, string.IsNullOrEmpty(value) ? null : value);
            }
        }

        /// <summary>
        /// 新增與修改共用的欄位設定（供應商不在這裡改，報價不能換供應商）；呼叫前輸入已通過驗證，必填欄位不會是 null
        /// </summary>
        private static void Modify(SupplierPriceAddVM vm, t_4061SupplierPrice entity, DateTime now, int userId)
        {
            entity.No = vm.No ?? string.Empty;
            entity.Name = vm.Name ?? string.Empty;
            entity.Unit = vm.Unit ?? string.Empty;
            entity.Price = vm.Price ?? 0m;
            entity.Note = vm.Note;
            entity.UpdateTime = now;
            entity.UpdateUserId = userId;
        }

        #endregion
    }
}
