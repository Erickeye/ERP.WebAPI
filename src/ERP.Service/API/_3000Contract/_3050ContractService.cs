using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._3000Contract;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._3000Contract;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._3000Contract
{
    /// <summary>
    /// 合約單-服務
    /// </summary>
    public interface I_3050ContractService
    {
        /// <summary>
        /// 查詢合約單列表（預設依合約起日由新到舊、分頁）
        /// </summary>
        Task<ResultModel<PagedResult<ContractVM>>> Index(ContractSearchVM vm);

        /// <summary>
        /// 取得單張合約單
        /// </summary>
        Task<ResultModel<ContractVM>> Get(int id);

        /// <summary>
        /// 新增合約單（草稿）
        /// </summary>
        Task<ResultModel<ContractSaveResultVM>> Add(ContractAddVM vm);

        /// <summary>
        /// 修改合約單（僅草稿、已退回）
        /// </summary>
        Task<ResultModel<ContractSaveResultVM>> Edit(ContractAddVM vm);

        /// <summary>
        /// 刪除合約單（僅草稿）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);
    }

    public class _3050ContractService : I_3050ContractService
    {
        private const string SerialPrefix = "CT";
        private const string CompanyDisabledMessage = "所選簽約公司已停用，請改選其他公司";
        private const string SupplierDisabledMessage = "所選供應商已停用，請改選其他供應商";
        private const decimal MaxAmount = 9999999999.99m;

        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISerialService _serialService;

        public _3050ContractService(
            ERPDbContext db,
            ICurrentUserService currentUserService,
            ISerialService serialService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
        }

        #region 查詢

        public async Task<ResultModel<PagedResult<ContractVM>>> Index(ContractSearchVM vm)
        {
            var contracts = _db.t_3050Contract
                .Where(SearchExpressionBuilder.Build<t_3050Contract>(vm));

            // 預設依合約起日由新到舊，起日相同時新建立的在前
            IQueryable<ContractVM> query = QueryList(contracts)
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.Id);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                // 類型顯示與狀態顯示是輸出時才計算的欄位，不是資料庫欄位，不開放排序
                var sortColumns = SortHelper.GetColumns<ContractVM>();
                sortColumns.Remove(nameof(ContractVM.TypeDisplay));
                sortColumns.Remove(nameof(ContractVM.StatusDisplay));
                query = query.ApplySort(vm, sortColumns);
            }

            var pagedResult = await query.ToPagedResultAsync(vm);
            FillDisplayFields(pagedResult.Items);

            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<ContractVM>> Get(int id)
        {
            var data = await QueryList(_db.t_3050Contract)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            FillDisplayFields(new List<ContractVM> { data });
            return ResultModel.Ok(data);
        }

        private static IQueryable<ContractVM> QueryList(IQueryable<t_3050Contract> contracts)
        {
            return contracts
                .AsNoTracking()
                .Select(x => new ContractVM
                {
                    Id = x.Id,
                    No = x.No,
                    ContractNo = x.ContractNo,
                    Type = x.Type,
                    SupplierId = x.SupplierId,
                    CustomerId = x.CustomerId,
                    PartyName = x.Supplier != null ? x.Supplier.Name : x.Customer!.Name,
                    SupplierIsActive = x.Supplier != null ? x.Supplier.IsActive : (bool?)null,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.AttribName,
                    CompanyIsActive = x.Company.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Amount = x.Amount,
                    Remark = x.Remark,
                    HasAttachment = x.AttachmentPath != null,
                    Status = x.Status,
                    CreateTime = x.CreateTime,
                    CreateUserId = x.CreateUserId,
                    CreateUserName = x.CreateUser.Name,
                    UpdateTime = x.UpdateTime
                });
        }

        /// <summary>
        /// 填入輸出時才計算的顯示欄位：合約類型與合約單狀態的顯示文字
        /// </summary>
        private static void FillDisplayFields(List<ContractVM> items)
        {
            foreach (var item in items)
            {
                item.TypeDisplay = ((ContractType)item.Type).GetDisplayName();
                item.StatusDisplay = GetStatusDisplay(item.Status);
            }
        }

        #endregion

        #region 新增、修改、刪除

        public async Task<ResultModel<ContractSaveResultVM>> Add(ContractAddVM vm)
        {
            var now = DateTime.Now;
            var entity = new t_3050Contract
            {
                CreateTime = now,
                CreateUserId = _currentUserService.UserId,
                Status = (int)ContractStatus.草稿
            };

            var result = await ModifyAsync(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            entity.UpdateTime = now;
            entity.UpdateUserId = _currentUserService.UserId;
            entity.No = await _serialService.GenerateAsync(SerialPrefix);

            _db.t_3050Contract.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(new ContractSaveResultVM
            {
                Id = entity.Id,
                No = entity.No
            });
        }

        public async Task<ResultModel<ContractSaveResultVM>> Edit(ContractAddVM vm)
        {
            var entity = await _db.t_3050Contract
                .Where(x => x.Id == vm.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿與已退回可以編輯，簽核中、已核准、已作廢的內容不可再變動
            if (entity.Status != (int)ContractStatus.草稿 &&
                entity.Status != (int)ContractStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，不可編輯合約單");
            }

            var result = await ModifyAsync(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            entity.UpdateTime = DateTime.Now;
            entity.UpdateUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();

            return ResultModel.Ok(new ContractSaveResultVM
            {
                Id = entity.Id,
                No = entity.No
            });
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await _db.t_3050Contract
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿可以刪除；已退回保留紀錄
            if (entity.Status != (int)ContractStatus.草稿)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，只有草稿可以刪除");
            }

            _db.t_3050Contract.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.No);
        }

        /// <summary>
        /// 新增與修改共用：驗證輸入並設定單頭欄位。
        /// </summary>
        private async Task<ResultModel<string>> ModifyAsync(ContractAddVM vm, t_3050Contract entity)
        {
            // Controller 有 [ValidateModel] 擋必填；Service 仍自行檢查，避免空白字串或被其他呼叫端繞過
            var contractNo = vm.ContractNo?.Trim();
            if (string.IsNullOrEmpty(contractNo))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約編號必填");
            }

            if (contractNo.Length > 64)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約編號長度不可超過 64 個字元");
            }

            if (vm.Remark != null && vm.Remark.Length > 1024)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "備註長度不可超過 1024 個字元");
            }

            if (!vm.Type.HasValue || !Enum.IsDefined(vm.Type.Value))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約類型必填，且必須是供應商合約或客戶合約");
            }

            if (!vm.CompanyId.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "簽約公司必填");
            }

            if (!vm.StartDate.HasValue || !vm.EndDate.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約起日與迄日必填");
            }

            // 起日不設下限（允許補登已簽訂的舊合約），只檢查迄日不得早於起日
            if (vm.EndDate.Value.Date < vm.StartDate.Value.Date)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約迄日不得早於起日");
            }

            if (!vm.Amount.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約總金額必填");
            }

            var amount = vm.Amount.Value;
            if (amount <= 0)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約總金額必須大於 0");
            }

            if (decimal.Round(amount, 2) != amount)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約總金額最多兩位小數");
            }

            if (amount > MaxAmount)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約總金額超過可記錄的上限");
            }

            var partyResult = await ValidatePartyAsync(vm, entity);
            if (!partyResult)
            {
                return partyResult;
            }

            // 簽約公司必填且必須存在；啟用中才可選。
            // 例外：編輯時維持原本的公司（即使它後來被停用）不阻擋儲存其他欄位，送簽時才擋停用的公司。
            var company = await _db.t_1080Company
                .AsNoTracking()
                .Where(x => x.Id == vm.CompanyId.Value)
                .Select(x => new { x.IsActive })
                .FirstOrDefaultAsync();
            if (company == null)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "找不到簽約公司");
            }

            if (!company.IsActive && entity.CompanyId != vm.CompanyId.Value)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, CompanyDisabledMessage);
            }

            // 合約編號在非作廢的合約單之間不可重複（不分類型；作廢單不佔用編號，編輯時排除自己）
            var duplicated = await _db.t_3050Contract
                .AsNoTracking()
                .AnyAsync(x => x.ContractNo == contractNo &&
                               x.Status != (int)ContractStatus.已作廢 &&
                               x.Id != entity.Id);
            if (duplicated)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約編號已存在，請確認是否重複登錄");
            }

            entity.ContractNo = contractNo;
            entity.Type = (int)vm.Type.Value;
            entity.SupplierId = vm.SupplierId;
            entity.CustomerId = vm.CustomerId;
            entity.CompanyId = vm.CompanyId.Value;
            entity.StartDate = vm.StartDate.Value.Date;
            entity.EndDate = vm.EndDate.Value.Date;
            entity.Amount = amount;
            entity.Remark = vm.Remark;

            return ResultModel.Ok();
        }

        /// <summary>
        /// 檢查合約對象：依合約類型恰好填一個。供應商合約須有存在且啟用中的供應商，客戶合約須有存在的客戶（客戶沒有停用概念）。
        /// 編輯時維持原本的供應商（即使它後來被停用）不阻擋儲存其他欄位，送簽時才擋停用的供應商。
        /// </summary>
        private async Task<ResultModel<string>> ValidatePartyAsync(ContractAddVM vm, t_3050Contract entity)
        {
            if (vm.Type == ContractType.供應商合約)
            {
                if (!vm.SupplierId.HasValue)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "供應商合約必須選擇供應商");
                }

                if (vm.CustomerId.HasValue)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "供應商合約不可填客戶");
                }

                var supplier = await _db.t_4060Supplier
                    .AsNoTracking()
                    .Where(x => x.Id == vm.SupplierId.Value)
                    .Select(x => new { x.IsActive })
                    .FirstOrDefaultAsync();
                if (supplier == null)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "找不到供應商");
                }

                if (!supplier.IsActive && entity.SupplierId != vm.SupplierId.Value)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, SupplierDisabledMessage);
                }

                return ResultModel.Ok();
            }

            if (!vm.CustomerId.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "客戶合約必須選擇客戶");
            }

            if (vm.SupplierId.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "客戶合約不可填供應商");
            }

            var customerExists = await _db.t_2000Customer.AnyAsync(x => x.Id == vm.CustomerId.Value);
            if (!customerExists)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "找不到客戶");
            }

            return ResultModel.Ok();
        }

        #endregion

        private static string GetStatusDisplay(int status)
        {
            return ((ContractStatus)status).GetDisplayName();
        }
    }
}
