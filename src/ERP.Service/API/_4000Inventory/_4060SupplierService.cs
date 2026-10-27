using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    /// <summary>
    /// 供應商-服務
    /// </summary>
    public interface I_4060SupplierService
    {
        /// <summary>
        /// 取得供應商下拉選單（只列啟用中的供應商；保留給 SharedController 既有呼叫，新程式請用 GetSelect）
        /// </summary>
        Task<ResultModel<ListResult<SelectModel>>> GetSupplierSelect();

        /// <summary>
        /// 取得填單用的供應商選單：只列啟用中的供應商；指定 includeId 時，該供應商即使已停用也一併回傳（標示已停用），供編輯草稿顯示原值
        /// </summary>
        Task<ResultModel<ListResult<SupplierSelectVM>>> GetSelect(int? includeId);

        /// <summary>
        /// 查詢供應商列表（分頁；可依關鍵字與啟用狀態篩選，預設啟用中的排在前面，再依供應商編號）
        /// </summary>
        Task<ResultModel<PagedResult<SupplierVM>>> Index(SupplierSearchVM vm);

        /// <summary>
        /// 取得單筆供應商
        /// </summary>
        Task<ResultModel<SupplierVM>> Get(int id);

        /// <summary>
        /// 新增供應商（預設為啟用中；供應商編號與統一編號不可與其他供應商重複，停用的也算）
        /// </summary>
        Task<ResultModel<int>> Add(SupplierAddVM vm);

        /// <summary>
        /// 修改供應商（含已被進貨單或庫存項目引用的供應商，編號與統編仍可改；不改變啟用狀態）
        /// </summary>
        Task<ResultModel<string>> Edit(SupplierAddVM vm);

        /// <summary>
        /// 停用供應商（停用後不再出現在新進貨單的選項；不限制最後一間）
        /// </summary>
        Task<ResultModel<string>> Disable(int id);

        /// <summary>
        /// 重新啟用供應商
        /// </summary>
        Task<ResultModel<string>> Enable(int id);

        /// <summary>
        /// 刪除供應商（只有沒被任何進貨單或庫存項目引用過的供應商能刪；不因存在報價而擋下，其供應商報價會一併刪除）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);
    }

    public class _4060SupplierService : I_4060SupplierService
    {
        private readonly ERPDbContext _db;

        public _4060SupplierService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<ListResult<SelectModel>>> GetSupplierSelect()
        {
            var data = await _db.t_4060Supplier
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new SelectModel
                {
                    Value = x.Id,
                    Text = x.Name ?? string.Empty
                })
                .ToListAsync();

            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<ListResult<SupplierSelectVM>>> GetSelect(int? includeId)
        {
            var rows = await _db.t_4060Supplier
                .AsNoTracking()
                .Where(x => x.IsActive || x.Id == includeId)
                .OrderBy(x => x.No)
                .ThenBy(x => x.Id)
                .Select(x => new { x.Id, x.No, x.Name, x.IsActive })
                .ToListAsync();

            // 文字在記憶體組合：同時顯示編號與名稱，沒有編號的舊資料只顯示名稱
            var items = rows
                .Select(x => new SupplierSelectVM
                {
                    Value = x.Id,
                    Text = string.IsNullOrWhiteSpace(x.No) ? x.Name : $"{x.No} {x.Name}",
                    IsActive = x.IsActive
                })
                .ToList();

            return ResultModel.Ok(items);
        }

        public async Task<ResultModel<PagedResult<SupplierVM>>> Index(SupplierSearchVM vm)
        {
            var query = _db.t_4060Supplier
                .AsNoTracking()
                .Where(SearchExpressionBuilder.Build<t_4060Supplier>(vm));

            // 關鍵字同時比對供應商編號、名稱與統一編號
            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var keyword = vm.Keyword.Trim();
                query = query.Where(x => x.No!.Contains(keyword) ||
                                         x.Name.Contains(keyword) ||
                                         x.TaxIdNumber!.Contains(keyword));
            }

            // 預設啟用中的排前面，再依供應商編號，最後依建立順序
            IQueryable<SupplierVM> list = query
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.No)
                .ThenBy(x => x.Id)
                .Select(x => new SupplierVM
                {
                    Id = x.Id,
                    No = x.No,
                    Name = x.Name,
                    TaxIdNumber = x.TaxIdNumber,
                    Owner = x.Owner,
                    ContactPhone = x.ContactPhone,
                    FaxPhone = x.FaxPhone,
                    Salesperson = x.Salesperson,
                    Address = x.Address,
                    BankName = x.BankName,
                    SubBankName = x.SubBankName,
                    CheckingAccount = x.CheckingAccount,
                    RemittanceAccount = x.RemittanceAccount,
                    PayDays = x.PayDays,
                    CreditLine = x.CreditLine,
                    IsActive = x.IsActive
                });

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                list = list.ApplySort(vm, SortHelper.GetColumns<SupplierVM>());
            }

            var pagedResult = await list.ToPagedResultAsync(vm);
            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<SupplierVM>> Get(int id)
        {
            var data = await _db.t_4060Supplier
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SupplierVM
                {
                    Id = x.Id,
                    No = x.No,
                    Name = x.Name,
                    TaxIdNumber = x.TaxIdNumber,
                    Owner = x.Owner,
                    ContactPhone = x.ContactPhone,
                    FaxPhone = x.FaxPhone,
                    Salesperson = x.Salesperson,
                    Address = x.Address,
                    BankName = x.BankName,
                    SubBankName = x.SubBankName,
                    CheckingAccount = x.CheckingAccount,
                    RemittanceAccount = x.RemittanceAccount,
                    PayDays = x.PayDays,
                    CreditLine = x.CreditLine,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<int>> Add(SupplierAddVM vm)
        {
            var checkResult = await ValidateAsync(vm, ignoreId: null);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            var entity = new t_4060Supplier
            {
                // 新增的供應商一律啟用中，才能立刻被進貨單選用
                IsActive = true
            };
            Modify(vm, entity);

            _db.t_4060Supplier.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.Id);
        }

        public async Task<ResultModel<string>> Edit(SupplierAddVM vm)
        {
            var entity = await _db.t_4060Supplier.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            var checkResult = await ValidateAsync(vm, ignoreId: entity.Id);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            // 依 ADR-0003，單據只記供應商外鍵；改名視為更正，所以被引用後編號與統編仍可修改
            Modify(vm, entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<string>> Disable(int id)
        {
            var entity = await _db.t_4060Supplier.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 供應商只影響新進貨單，不像公司需要保留最後一間，所以停用沒有額外限制
            entity.IsActive = false;
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<string>> Enable(int id)
        {
            var entity = await _db.t_4060Supplier.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            entity.IsActive = true;
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await _db.t_4060Supplier.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (await IsReferencedAsync(id))
            {
                return ResultModel.Error(ErrorCodeType.InvalidRequest, "供應商已被進貨單、庫存項目或合約單引用，無法刪除，請改用停用");
            }

            // 資料庫外鍵雖設連帶刪除，但不依賴它（測試環境的 SQLite 不會連帶刪），由服務層明確刪除並同一交易儲存
            var prices = await _db.t_4061SupplierPrice.Where(x => x.SupplierId == id).ToListAsync();
            _db.t_4061SupplierPrice.RemoveRange(prices);
            _db.t_4060Supplier.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        #region 共用驗證與欄位設定

        /// <summary>
        /// 欄位驗證（必填、長度、格式沿用輸入 VM 的 DataAnnotations，Controller 與 Service 共用同一組規則）加上編號與統編唯一檢查。
        /// 會先去除字串前後空白，並把空白字串視為未填。
        /// </summary>
        /// <param name="ignoreId">編輯時排除自己，新增傳 null</param>
        private async Task<ResultModel<string>> ValidateAsync(SupplierAddVM vm, int? ignoreId)
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
                        : typeof(SupplierAddVM).GetProperty(member)?.GetCustomAttribute<DisplayAttribute>()?.Name;
                    return displayName == null ? r.ErrorMessage : $"{displayName}：{r.ErrorMessage}";
                });
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, string.Join("；", messages));
            }

            // 唯一索引會擋，但先檢查才能給明確的訊息；名稱允許重複，不檢查
            if (await _db.t_4060Supplier.AnyAsync(x => x.No == vm.No && x.Id != ignoreId))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "供應商編號已被其他供應商使用");
            }
            if (vm.TaxIdNumber != null &&
                await _db.t_4060Supplier.AnyAsync(x => x.TaxIdNumber == vm.TaxIdNumber && x.Id != ignoreId))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "統一編號已被其他供應商使用");
            }

            return ResultModel.Ok();
        }

        private static void NormalizeStrings(SupplierAddVM vm)
        {
            foreach (var property in typeof(SupplierAddVM).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var value = ((string?)property.GetValue(vm))?.Trim();
                property.SetValue(vm, string.IsNullOrEmpty(value) ? null : value);
            }
        }

        /// <summary>
        /// 新增與修改共用的欄位設定（不含 IsActive，啟用與停用由專屬動作處理；也不碰信用餘額、最後交易日、暫付金額）；
        /// 呼叫前輸入已通過驗證，必填欄位不會是 null
        /// </summary>
        private static void Modify(SupplierAddVM vm, t_4060Supplier entity)
        {
            entity.No = vm.No;
            entity.Name = vm.Name ?? string.Empty;
            entity.TaxIdNumber = vm.TaxIdNumber;
            entity.Owner = vm.Owner;
            entity.ContactPhone = vm.ContactPhone;
            entity.FaxPhone = vm.FaxPhone;
            entity.Salesperson = vm.Salesperson;
            entity.Address = vm.Address;
            entity.BankName = vm.BankName;
            entity.SubBankName = vm.SubBankName;
            entity.CheckingAccount = vm.CheckingAccount;
            entity.RemittanceAccount = vm.RemittanceAccount;
            entity.PayDays = vm.PayDays;
            entity.CreditLine = vm.CreditLine;
        }

        /// <summary>
        /// 供應商是否被進貨單或庫存項目引用過
        /// </summary>
        private async Task<bool> IsReferencedAsync(int id)
        {
            return await _db.t_4010Purchase.AnyAsync(x => x.SupplierId == id)
                || await _db.t_4000Inventory.AnyAsync(x => x.SupplierId == id)
                || await _db.t_3050Contract.AnyAsync(x => x.SupplierId == id);
        }

        #endregion
    }
}
