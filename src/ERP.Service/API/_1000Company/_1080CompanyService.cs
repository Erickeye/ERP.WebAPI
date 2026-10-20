using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._1000Company
{
    /// <summary>
    /// 公司-服務
    /// </summary>
    public interface I_1080CompanyService
    {
        /// <summary>
        /// 查詢公司列表（分頁；可依關鍵字與啟用狀態篩選，預設啟用中的排在前面）
        /// </summary>
        Task<ResultModel<PagedResult<CompanyVM>>> Index(CompanySearchVM vm);

        /// <summary>
        /// 取得填單用的公司選單：只列啟用中的公司；指定 includeId 時，該公司即使已停用也一併回傳（標示已停用），供編輯草稿顯示原值
        /// </summary>
        Task<ResultModel<ListResult<CompanySelectVM>>> GetSelect(int? includeId);

        /// <summary>
        /// 新增公司（預設為啟用中；公司簡稱與統一編號不可與其他公司重複，停用的公司也算）
        /// </summary>
        Task<ResultModel<int>> Add(CompanyAddVM vm);

        /// <summary>
        /// 取得單筆公司
        /// </summary>
        Task<ResultModel<CompanyVM>> Get(int id);

        /// <summary>
        /// 修改公司（含已被單據引用的公司，簡稱與統編仍可改；不改變啟用狀態）
        /// </summary>
        Task<ResultModel<string>> Edit(CompanyAddVM vm);

        /// <summary>
        /// 停用公司（停用後不再出現在填單選項；至少要保留一間啟用中的公司）
        /// </summary>
        Task<ResultModel<string>> Disable(int id);

        /// <summary>
        /// 重新啟用公司
        /// </summary>
        Task<ResultModel<string>> Enable(int id);

        /// <summary>
        /// 刪除公司（只有沒被任何單據引用過的公司能刪；啟用中的最後一間不可刪）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);
    }

    public class _1080CompanyService : I_1080CompanyService
    {
        private readonly ERPDbContext _db;

        public _1080CompanyService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PagedResult<CompanyVM>>> Index(CompanySearchVM vm)
        {
            var query = _db.t_1080Company
                .AsNoTracking()
                .Where(SearchExpressionBuilder.Build<t_1080Company>(vm));

            // 關鍵字同時比對簡稱、全名與統一編號
            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var keyword = vm.Keyword.Trim();
                query = query.Where(x => x.AttribName.Contains(keyword) ||
                                         x.Name.Contains(keyword) ||
                                         x.TaxInvoiceNumber.Contains(keyword));
            }

            // 預設啟用中的排前面，同狀態依建立順序
            IQueryable<CompanyVM> list = query
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Id)
                .Select(x => new CompanyVM
                {
                    Id = x.Id,
                    AttribName = x.AttribName,
                    Name = x.Name,
                    TaxInvoiceNumber = x.TaxInvoiceNumber,
                    TaxSerialNumber = x.TaxSerialNumber,
                    Owner = x.Owner,
                    ContactPhone = x.ContactPhone,
                    FaxPhone = x.FaxPhone,
                    RegisteredAddress = x.RegisteredAddress,
                    DeliveryAddress = x.DeliveryAddress,
                    TaxInvoiceAddress = x.TaxInvoiceAddress,
                    BankName = x.BankName,
                    CheckingAccount = x.CheckingAccount,
                    RemittanceAccount = x.RemittanceAccount,
                    PayDays = x.PayDays,
                    FoundedDate = x.FoundedDate,
                    InvoiceForm = x.InvoiceForm,
                    IsActive = x.IsActive
                });

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                list = list.ApplySort(vm, SortHelper.GetColumns<CompanyVM>());
            }

            var pagedResult = await list.ToPagedResultAsync(vm);
            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<ListResult<CompanySelectVM>>> GetSelect(int? includeId)
        {
            var items = await _db.t_1080Company
                .AsNoTracking()
                .Where(x => x.IsActive || x.Id == includeId)
                .OrderBy(x => x.Id)
                .Select(x => new CompanySelectVM
                {
                    Value = x.Id,
                    Text = x.AttribName,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return ResultModel.Ok(items);
        }

        public async Task<ResultModel<int>> Add(CompanyAddVM vm)
        {
            var checkResult = await ValidateAsync(vm, ignoreId: null);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            var entity = new t_1080Company
            {
                // 新增的公司一律啟用中，才能立刻被填單選用
                IsActive = true
            };
            Modify(vm, entity);

            _db.t_1080Company.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.Id);
        }

        public async Task<ResultModel<CompanyVM>> Get(int id)
        {
            var data = await _db.t_1080Company
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CompanyVM
                {
                    Id = x.Id,
                    AttribName = x.AttribName,
                    Name = x.Name,
                    TaxInvoiceNumber = x.TaxInvoiceNumber,
                    TaxSerialNumber = x.TaxSerialNumber,
                    Owner = x.Owner,
                    ContactPhone = x.ContactPhone,
                    FaxPhone = x.FaxPhone,
                    RegisteredAddress = x.RegisteredAddress,
                    DeliveryAddress = x.DeliveryAddress,
                    TaxInvoiceAddress = x.TaxInvoiceAddress,
                    BankName = x.BankName,
                    CheckingAccount = x.CheckingAccount,
                    RemittanceAccount = x.RemittanceAccount,
                    PayDays = x.PayDays,
                    FoundedDate = x.FoundedDate,
                    InvoiceForm = x.InvoiceForm,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<string>> Edit(CompanyAddVM vm)
        {
            var entity = await _db.t_1080Company.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            var checkResult = await ValidateAsync(vm, ignoreId: entity.Id);
            if (!checkResult)
            {
                return ResultModel.Error(checkResult.ErrorCode, checkResult.ErrorMessage);
            }

            // 依 ADR-0001，單據只記公司外鍵；改名視為更正，所以被引用後簡稱與統編仍可修改
            Modify(vm, entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<string>> Disable(int id)
        {
            var entity = await _db.t_1080Company.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 已經是停用狀態就不必再動，也不受「最後一間」限制
            if (!entity.IsActive)
            {
                return ResultModel.Ok();
            }

            if (!await HasOtherActiveCompanyAsync(id))
            {
                return ResultModel.Error(ErrorCodeType.InvalidRequest, "至少要保留一間啟用中的公司，無法停用最後一間");
            }

            entity.IsActive = false;
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        public async Task<ResultModel<string>> Enable(int id)
        {
            var entity = await _db.t_1080Company.FirstOrDefaultAsync(x => x.Id == id);
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
            var entity = await _db.t_1080Company.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (await IsReferencedAsync(id))
            {
                return ResultModel.Error(ErrorCodeType.InvalidRequest, "公司已被單據引用，無法刪除，請改用停用");
            }

            // 與停用同一個保護：不能讓系統一間可選的公司都沒有
            if (entity.IsActive && !await HasOtherActiveCompanyAsync(id))
            {
                return ResultModel.Error(ErrorCodeType.InvalidRequest, "至少要保留一間啟用中的公司，無法刪除最後一間");
            }

            _db.t_1080Company.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok();
        }

        #region 共用驗證與欄位設定

        /// <summary>
        /// 欄位驗證（必填、長度、格式沿用輸入 VM 的 DataAnnotations，Controller 與 Service 共用同一組規則）加上簡稱與統編唯一檢查。
        /// 會先去除字串前後空白，並把空白字串視為未填。
        /// </summary>
        /// <param name="ignoreId">編輯時排除自己，新增傳 null</param>
        private async Task<ResultModel<string>> ValidateAsync(CompanyAddVM vm, int? ignoreId)
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
                        : typeof(CompanyAddVM).GetProperty(member)?.GetCustomAttribute<DisplayAttribute>()?.Name;
                    return displayName == null ? r.ErrorMessage : $"{displayName}：{r.ErrorMessage}";
                });
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, string.Join("；", messages));
            }

            // 唯一索引會擋，但先檢查才能給明確的訊息
            if (await _db.t_1080Company.AnyAsync(x => x.TaxInvoiceNumber == vm.TaxInvoiceNumber && x.Id != ignoreId))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "統一編號已被其他公司使用");
            }
            if (await _db.t_1080Company.AnyAsync(x => x.AttribName == vm.AttribName && x.Id != ignoreId))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "公司簡稱已被其他公司使用");
            }

            return ResultModel.Ok();
        }

        private static void NormalizeStrings(CompanyAddVM vm)
        {
            foreach (var property in typeof(CompanyAddVM).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                var value = ((string?)property.GetValue(vm))?.Trim();
                property.SetValue(vm, string.IsNullOrEmpty(value) ? null : value);
            }
        }

        /// <summary>
        /// 新增與修改共用的欄位設定（不含 IsActive，啟用與停用由專屬動作處理）；呼叫前輸入已通過驗證，必填欄位不會是 null
        /// </summary>
        private static void Modify(CompanyAddVM vm, t_1080Company entity)
        {
            entity.AttribName = vm.AttribName ?? string.Empty;
            entity.Name = vm.Name ?? string.Empty;
            entity.TaxInvoiceNumber = vm.TaxInvoiceNumber ?? string.Empty;
            entity.TaxSerialNumber = vm.TaxSerialNumber ?? string.Empty;
            entity.Owner = vm.Owner ?? string.Empty;
            entity.ContactPhone = vm.ContactPhone;
            entity.FaxPhone = vm.FaxPhone;
            entity.RegisteredAddress = vm.RegisteredAddress ?? string.Empty;
            entity.DeliveryAddress = vm.DeliveryAddress;
            entity.TaxInvoiceAddress = vm.TaxInvoiceAddress;
            entity.BankName = vm.BankName;
            entity.CheckingAccount = vm.CheckingAccount;
            entity.RemittanceAccount = vm.RemittanceAccount;
            entity.PayDays = vm.PayDays;
            entity.FoundedDate = vm.FoundedDate;
            entity.InvoiceForm = vm.InvoiceForm;
        }

        /// <summary>
        /// 除了指定的公司以外，是否還有啟用中的公司
        /// </summary>
        private Task<bool> HasOtherActiveCompanyAsync(int id)
        {
            return _db.t_1080Company.AnyAsync(x => x.IsActive && x.Id != id);
        }

        /// <summary>
        /// 公司是否被任何單據引用過。
        /// 目前已納入領料單；出差單加上公司欄位時（ticket 03）要在這裡補上對應的查詢。
        /// </summary>
        private async Task<bool> IsReferencedAsync(int id)
        {
            return await _db.t_4030MaterialIssue.AnyAsync(x => x.CompanyId == id);
        }

        #endregion
    }
}
