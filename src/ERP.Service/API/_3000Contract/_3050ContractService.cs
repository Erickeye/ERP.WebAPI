using ERP.Approval.Abstractions;
using ERP.Approval.Services;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._3000Contract;
using ERP.Library.Enums.Login;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._3000Contract;
using ERP.Service.API.AMS;
using ERP.Service.Sftp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// 上傳或更換合約掃描檔（僅草稿、已退回；只接受 PDF，不超過 10 MB；存 SFTP，資料表只存遠端路徑）
        /// </summary>
        Task<ResultModel<string>> UploadAttachment(ContractAttachmentVM vm);

        /// <summary>
        /// 下載合約掃描檔
        /// </summary>
        Task<ResultModel<ContractAttachmentFileVM>> DownloadAttachment(int id);

        /// <summary>
        /// 送出簽核（草稿、已退回；送簽時必須已上傳合約掃描檔，並重新檢查供應商與簽約公司未停用）
        /// </summary>
        Task<ResultModel<string>> SendApproval(ApprovalVM vm);

        /// <summary>
        /// 核准（只有簽核中；最後一關核准後單據變成已核准）
        /// </summary>
        Task<ResultModel<string>> Approval(ApprovalVM vm);

        /// <summary>
        /// 拒絕（只有簽核中；必須填原因，成功後單據變成已退回）
        /// </summary>
        Task<ResultModel<string>> RejectApproval(ApprovalVM vm);

        /// <summary>
        /// 撤銷（只有簽核中；限送簽人；成功後單據變成已退回）
        /// </summary>
        Task<ResultModel<string>> RevokeApproval(ApprovalVM vm);

        /// <summary>
        /// 作廢（只有已核准；限最後一關簽核人；成功後單據變成已作廢，為終態）
        /// </summary>
        Task<ResultModel<string>> VoidApproval(ApprovalVM vm);
    }

    public class _3050ContractService : I_3050ContractService
    {
        private const string SerialPrefix = "CT";
        private const string CompanyDisabledMessage = "所選簽約公司已停用，請改選其他公司";
        private const string SupplierDisabledMessage = "所選供應商已停用，請改選其他供應商";
        private const decimal MaxAmount = 9999999999.99m;

        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private const long MaxAttachmentBytes = 10 * 1024 * 1024;
        private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();

        private readonly ISerialService _serialService;
        private readonly ISftpService _sftpService;
        private readonly ILogger<_3050ContractService> _logger;
        private readonly IApprovalWorkflowService _approvalWorkflowService;

        public _3050ContractService(
            ERPDbContext db,
            ICurrentUserService currentUserService,
            ISerialService serialService,
            ISftpService sftpService,
            ILogger<_3050ContractService> logger,
            IApprovalWorkflowService approvalWorkflowService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
            _sftpService = sftpService;
            _logger = logger;
            _approvalWorkflowService = approvalWorkflowService;
        }

        #region 查詢

        public async Task<ResultModel<PagedResult<ContractVM>>> Index(ContractSearchVM vm)
        {
            if (vm.PeriodStart.HasValue && vm.PeriodEnd.HasValue &&
                vm.PeriodEnd.Value.Date < vm.PeriodStart.Value.Date)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約期間結束不得早於開始");
            }

            var scope = await GetReadableScopeAsync();
            if (!scope)
            {
                return ResultModel.Error(scope.ErrorCode, scope.ErrorMessage);
            }
            var pendingNos = scope.Data!.PendingNos;

            var contracts = scope.Data.Contracts;
            if (vm.PendingMyApproval)
            {
                // 「待我簽核」取自引擎的待簽核查詢，並限定簽核中的合約單
                contracts = contracts.Where(x => x.Status == (int)ContractStatus.簽核中 && pendingNos.Contains(x.No));
            }

            // 篩選條件只在可見範圍內再縮小，不會擴大可見範圍
            contracts = contracts.Where(SearchExpressionBuilder.Build<t_3050Contract>(vm));

            if (vm.SupplierId.HasValue)
            {
                var supplierId = vm.SupplierId.Value;
                contracts = contracts.Where(x => x.SupplierId == supplierId);
            }
            if (vm.CustomerId.HasValue)
            {
                var customerId = vm.CustomerId.Value;
                contracts = contracts.Where(x => x.CustomerId == customerId);
            }

            if (!string.IsNullOrWhiteSpace(vm.Keyword))
            {
                var keyword = vm.Keyword.Trim();
                contracts = contracts.Where(x => x.No.Contains(keyword) || x.ContractNo.Contains(keyword));
            }

            // 生效中與已到期只存在於已核准的合約單，起訖日都含當天
            var today = DateTime.Today;
            if (vm.Validity == ContractValidity.生效中)
            {
                contracts = contracts.Where(x => x.Status == (int)ContractStatus.已核准 &&
                                                 x.StartDate <= today && x.EndDate >= today);
            }
            else if (vm.Validity == ContractValidity.已到期)
            {
                contracts = contracts.Where(x => x.Status == (int)ContractStatus.已核准 && x.EndDate < today);
            }

            // 日期區間以「與合約起訖有交集」為準：合約迄日不早於區間開始，且合約起日不晚於區間結束（邊界日算有交集）
            if (vm.PeriodStart.HasValue)
            {
                var periodStart = vm.PeriodStart.Value.Date;
                contracts = contracts.Where(x => x.EndDate >= periodStart);
            }
            if (vm.PeriodEnd.HasValue)
            {
                var periodEnd = vm.PeriodEnd.Value.Date;
                contracts = contracts.Where(x => x.StartDate <= periodEnd);
            }

            // 預設依合約起日由新到舊，起日相同時新建立的在前
            IQueryable<ContractVM> query = QueryList(contracts)
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.Id);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                // 顯示文字、有效性與目前使用者能做的動作是輸出時才計算的欄位，不是資料庫欄位，不開放排序
                var sortColumns = SortHelper.GetColumns<ContractVM>();
                sortColumns.Remove(nameof(ContractVM.TypeDisplay));
                sortColumns.Remove(nameof(ContractVM.StatusDisplay));
                sortColumns.Remove(nameof(ContractVM.Validity));
                sortColumns.Remove(nameof(ContractVM.ValidityDisplay));
                sortColumns.Remove(nameof(ContractVM.CanApprove));
                sortColumns.Remove(nameof(ContractVM.CanEdit));
                sortColumns.Remove(nameof(ContractVM.CanDelete));
                sortColumns.Remove(nameof(ContractVM.CanRevoke));
                sortColumns.Remove(nameof(ContractVM.CanVoid));
                query = query.ApplySort(vm, sortColumns);
            }

            var pagedResult = await query.ToPagedResultAsync(vm);
            await FillComputedFieldsAsync(pagedResult.Items, pendingNos);

            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<ContractVM>> Get(int id)
        {
            var scope = await GetReadableScopeAsync();
            if (!scope)
            {
                return ResultModel.Error(scope.ErrorCode, scope.ErrorMessage);
            }

            var data = await QueryList(scope.Data!.Contracts)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            await FillComputedFieldsAsync(new List<ContractVM> { data }, scope.Data.PendingNos);
            return ResultModel.Ok(data);
        }

        /// <summary>
        /// 目前使用者可讀取的合約單範圍，連同輪到自己簽核的單號。列表、單筆與下載一律從這裡取得查詢起點，
        /// 避免某個端點漏掉可見範圍限縮而變成資料隔離的缺口。
        /// </summary>
        private sealed class ReadableScope
        {
            public IQueryable<t_3050Contract> Contracts { get; init; } = null!;

            public List<string> PendingNos { get; init; } = new();
        }

        private async Task<ResultModel<ReadableScope>> GetReadableScopeAsync()
        {
            var pending = await GetMyPendingContractNosAsync();
            if (!pending)
            {
                return ResultModel.Error(pending.ErrorCode, pending.ErrorMessage);
            }

            return ResultModel.Ok(new ReadableScope
            {
                Contracts = await ReadableContractsAsync(pending.Data!),
                PendingNos = pending.Data!
            });
        }

        /// <summary>
        /// 列表、單筆與下載的讀取範圍：有「合約單檢視」權限者看全部；其餘只看自己建立的，
        /// 加上簽核人身分的合約單——輪到自己簽核的，或簽核紀錄中有自己（指定人員）／自己的角色正在待簽（指定角色）的。
        /// 簽核人不必有檢視權限，所以簽核人與建單人才看得到各自相關的單；簽核動作（核准、拒絕、作廢）由引擎依簽核紀錄判斷，不看這個範圍。
        /// </summary>
        private async Task<IQueryable<t_3050Contract>> ReadableContractsAsync(List<string> pendingNos)
        {
            if (await HasPermissionAsync(PermissionType.合約單檢視))
            {
                return _db.t_3050Contract;
            }

            var userId = _currentUserService.UserId;
            var roleId = _currentUserService.RoleId;
            var tableType = (int)TableType.合約單;

            // 送簽時引擎會建立一筆 StepOrder 為 0 的紀錄並帶送簽人的角色，不能拿它當簽核人依據，否則同角色的人都看得到
            var signingRecords = _db.ApprovalRecord
                .Where(r => r.TableType == tableType &&
                            r.StepOrder >= 1 &&
                            (r.UserId == userId ||
                             (r.RoleId == roleId && r.Status == (int)ApprovalStatus.Pending)));

            return _db.t_3050Contract
                .Where(x => x.CreateUserId == userId ||
                            pendingNos.Contains(x.No) ||
                            signingRecords.Any(r => r.TableId == x.No));
        }

        /// <summary>
        /// 目前使用者的角色是否有指定功能權限（與登入時寫入 JWT 的來源相同：Permission 表的角色與頁面識別碼）
        /// </summary>
        private Task<bool> HasPermissionAsync(PermissionType permission)
        {
            var roleId = _currentUserService.RoleId;
            var pageId = (int)permission;

            return _db.Permission.AnyAsync(x => x.RoleId == roleId && x.PageId == pageId);
        }

        /// <summary>
        /// 取得引擎「待簽核」查詢中，輪到目前使用者簽的合約單單號
        /// </summary>
        private async Task<ResultModel<List<string>>> GetMyPendingContractNosAsync()
        {
            var notify = await _approvalWorkflowService.GetApprovalNotify();
            if (!notify)
            {
                return ResultModel.Error(notify.ErrorCode, notify.ErrorMessage);
            }

            var nos = notify.Data!.Items
                .Where(x => x.TableType == (int)TableType.合約單 && x.TableId != null)
                .Select(x => x.TableId!)
                .Distinct()
                .ToList();
            return ResultModel<List<string>>.Ok(nos);
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
        /// 填入輸出時才計算的欄位：類型與狀態的顯示文字、有效性（生效中、已到期），
        /// 以及目前使用者能做的動作 CanApprove、CanEdit、CanDelete、CanRevoke、CanVoid（由後端判斷，前端不自行推算）
        /// </summary>
        private async Task FillComputedFieldsAsync(List<ContractVM> items, List<string> pendingNos)
        {
            if (items.Count == 0)
            {
                return;
            }

            var userId = _currentUserService.UserId;
            var hasEdit = await HasPermissionAsync(PermissionType.合約單編輯);
            var hasDelete = await HasPermissionAsync(PermissionType.合約單刪除);

            // 撤銷與作廢都看最近一輪的簽核紀錄
            var roundNos = items
                .Where(x => x.Status == (int)ContractStatus.簽核中 || x.Status == (int)ContractStatus.已核准)
                .Select(x => x.No!)
                .ToList();
            var rounds = await LoadLatestRoundsAsync(roundNos);
            var today = DateTime.Today;

            foreach (var item in items)
            {
                rounds.TryGetValue(item.No!, out var round);

                item.TypeDisplay = ((ContractType)item.Type).GetDisplayName();
                item.StatusDisplay = GetStatusDisplay(item.Status);

                var validity = GetValidity(item.Status, item.StartDate, item.EndDate, today);
                item.Validity = validity.HasValue ? (int)validity.Value : null;
                item.ValidityDisplay = validity?.GetDisplayName();

                item.CanApprove = item.Status == (int)ContractStatus.簽核中 && pendingNos.Contains(item.No!);
                item.CanEdit = hasEdit &&
                               (item.Status == (int)ContractStatus.草稿 || item.Status == (int)ContractStatus.已退回);
                item.CanDelete = hasDelete && item.Status == (int)ContractStatus.草稿;
                item.CanRevoke = hasEdit &&
                                 item.Status == (int)ContractStatus.簽核中 &&
                                 round?.SenderUserId == userId;
                item.CanVoid = item.Status == (int)ContractStatus.已核准 &&
                               round != null &&
                               round.LastStepUserIds.Contains(userId);
            }
        }

        /// <summary>
        /// 已核准的合約單依今天的日期計算有效性：起訖日都含當天；尚未到起日或不是已核准的沒有有效性
        /// </summary>
        private static ContractValidity? GetValidity(int status, DateTime startDate, DateTime endDate, DateTime today)
        {
            if (status != (int)ContractStatus.已核准)
            {
                return null;
            }

            if (endDate.Date < today)
            {
                return ContractValidity.已到期;
            }

            return startDate.Date <= today ? ContractValidity.生效中 : null;
        }

        /// <summary>
        /// 一輪送簽的簽核紀錄摘要：送簽人，以及最後一關（排除已撤銷）的簽核人
        /// </summary>
        private sealed class ApprovalRound
        {
            public int? SenderUserId { get; init; }

            public HashSet<int> LastStepUserIds { get; init; } = new();
        }

        /// <summary>
        /// 以單號取得最近一輪送簽的簽核紀錄摘要。一輪的界線是最新一筆 StepOrder 為 0 的送簽紀錄，
        /// 被拒絕或撤銷後重送時，舊一輪的紀錄不算（與引擎的撤銷、作廢檢查一致）。
        /// </summary>
        private async Task<Dictionary<string, ApprovalRound>> LoadLatestRoundsAsync(List<string> nos)
        {
            var rounds = new Dictionary<string, ApprovalRound>();
            if (nos.Count == 0)
            {
                return rounds;
            }

            var tableType = (int)TableType.合約單;
            var records = await _db.ApprovalRecord
                .AsNoTracking()
                .Where(r => r.TableType == tableType && r.TableId != null && nos.Contains(r.TableId))
                .Select(r => new { r.Id, TableId = r.TableId!, r.StepOrder, r.UserId, r.Status })
                .ToListAsync();

            foreach (var group in records.GroupBy(r => r.TableId))
            {
                var sendRecord = group
                    .Where(r => r.StepOrder == 0)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefault();
                var round = group
                    .Where(r => r.Id >= (sendRecord?.Id ?? 0) && r.Status != (int)ApprovalStatus.GetRevoked)
                    .ToList();
                var lastStepOrder = round.Count == 0 ? (int?)null : round.Max(r => r.StepOrder);

                rounds[group.Key] = new ApprovalRound
                {
                    SenderUserId = sendRecord?.UserId,
                    LastStepUserIds = round
                        .Where(r => r.StepOrder == lastStepOrder && r.UserId.HasValue)
                        .Select(r => r.UserId!.Value)
                        .ToHashSet()
                };
            }

            return rounds;
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

        #region 合約掃描檔

        public async Task<ResultModel<string>> UploadAttachment(ContractAttachmentVM vm)
        {
            var entity = await _db.t_3050Contract
                .Where(x => x.Id == vm.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿與已退回可以上傳或更換，簽核中、已核准、已作廢的掃描檔不可再變動
            if (entity.Status != (int)ContractStatus.草稿 &&
                entity.Status != (int)ContractStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，不可上傳或更換合約掃描檔");
            }

            var file = vm.File;
            if (file == null || file.Length == 0)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "請選擇要上傳的合約掃描檔");
            }

            if (file.Length > MaxAttachmentBytes)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約掃描檔大小不可超過 10 MB");
            }

            // 副檔名、內容類型與檔頭三者都要是 PDF，避免只改副檔名的其他檔案混進來
            var isPdfName = string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase);
            var isPdfType = string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
            if (!isPdfName || !isPdfType)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約掃描檔只接受 PDF 檔案");
            }

            await using var stream = file.OpenReadStream();
            var header = new byte[PdfSignature.Length];
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length));
            if (read != header.Length || !header.AsSpan().SequenceEqual(PdfSignature))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "合約掃描檔只接受 PDF 檔案");
            }
            stream.Position = 0;

            // 遠端路徑由系統產生（單號加亂數），不使用上傳者提供的檔名，避免互相覆蓋與路徑穿越
            var remotePath = $"/contract-{entity.No}-{Guid.NewGuid():N}.pdf";
            var upload = _sftpService.UploadStream(stream, remotePath);
            if (!upload)
            {
                return ResultModel.Error(upload.ErrorCode, upload.ErrorMessage);
            }

            var oldPath = entity.AttachmentPath;
            entity.AttachmentPath = remotePath;
            entity.UpdateTime = DateTime.Now;
            entity.UpdateUserId = _currentUserService.UserId;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                // 資料庫沒存成功就不留下剛上傳的檔案
                DeleteRemoteFile(remotePath);
                throw;
            }

            // 更換時清掉舊檔；刪除失敗只會留下孤兒檔案，不影響單據，所以不回報錯誤（只記 Log 供追查）
            if (!string.IsNullOrEmpty(oldPath))
            {
                DeleteRemoteFile(oldPath);
            }

            return ResultModel.Ok("合約掃描檔已上傳");
        }

        /// <summary>
        /// 刪除遠端的合約掃描檔；失敗不影響呼叫端流程，記錄警告以便追查孤兒檔案
        /// </summary>
        private void DeleteRemoteFile(string remotePath)
        {
            var result = _sftpService.DeleteFile(remotePath);
            if (!result)
            {
                _logger.LogWarning("刪除遠端合約掃描檔失敗 {RemotePath}：{ErrorMessage}", remotePath, result.ErrorMessage);
            }
        }

        public async Task<ResultModel<ContractAttachmentFileVM>> DownloadAttachment(int id)
        {
            // 下載與檢視同一個可見範圍：簽核人沒有檢視權限也能下載待簽核單的掃描檔
            var scope = await GetReadableScopeAsync();
            if (!scope)
            {
                return ResultModel.Error(scope.ErrorCode, scope.ErrorMessage);
            }

            var data = await scope.Data!.Contracts
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new { x.No, x.AttachmentPath })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (string.IsNullOrEmpty(data.AttachmentPath))
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "此合約單尚未上傳合約掃描檔");
            }

            var download = _sftpService.DownloadFile(data.AttachmentPath);
            if (!download || download.Data == null)
            {
                return ResultModel.Error(download.ErrorCode, download.ErrorMessage);
            }

            return ResultModel.Ok(new ContractAttachmentFileVM
            {
                Content = download.Data,
                FileName = $"{data.No}.pdf"
            });
        }

        #endregion

        #region 簽核動作（與單據狀態同一交易，見差勤 ADR 0001）

        public async Task<ResultModel<string>> SendApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.合約單;

            var contract = await GetContractByNoAsync(vm.TableId);
            if (contract == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 草稿與已退回可送簽；已作廢為終態，不可重送
            if (contract.Status != (int)ContractStatus.草稿 &&
                contract.Status != (int)ContractStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(contract.Status)}】，不可送出簽核");
            }

            // 簽核人要看掃描檔才能決定，所以沒上傳就不能送簽
            if (string.IsNullOrEmpty(contract.AttachmentPath))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "請先上傳合約掃描檔才能送出簽核");
            }

            // 草稿可能放了幾天，送簽時重新檢查簽約公司與供應商是否仍啟用；送簽之後的簽核、核准、撤銷、作廢不受停用影響
            var companyActive = await _db.t_1080Company
                .AsNoTracking()
                .AnyAsync(x => x.Id == contract.CompanyId && x.IsActive);
            if (!companyActive)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, CompanyDisabledMessage);
            }

            if (contract.SupplierId.HasValue)
            {
                var supplierActive = await _db.t_4060Supplier
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == contract.SupplierId.Value && x.IsActive);
                if (!supplierActive)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, SupplierDisabledMessage);
                }
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.SendApprovalProcess(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                contract.Status = (int)ContractStatus.簽核中;
                contract.UpdateTime = DateTime.Now;
                contract.UpdateUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> Approval(ApprovalVM vm)
        {
            vm.TableType = TableType.合約單;

            var contract = await GetContractByNoAsync(vm.TableId);
            if (contract == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (contract.Status != (int)ContractStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(contract.Status)}】，不可簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                // 是否輪到目前使用者由引擎判斷，錯誤原樣轉出
                var result = await _approvalWorkflowService.Approval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 以引擎回報流程完成為準；中間關卡或同關尚有人未核准時維持簽核中
                if (result.Data == ApprovalWorkflowService.CompletedMessage)
                {
                    contract.Status = (int)ContractStatus.已核准;
                    contract.UpdateTime = DateTime.Now;
                    contract.UpdateUserId = _currentUserService.UserId;
                    await _db.SaveChangesAsync();
                }

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RejectApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.合約單;

            var contract = await GetContractByNoAsync(vm.TableId);
            if (contract == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (contract.Status != (int)ContractStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(contract.Status)}】，不可拒絕簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                // 拒絕原因必填由引擎檢查，錯誤原樣轉出
                var result = await _approvalWorkflowService.RejectApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                contract.Status = (int)ContractStatus.已退回;
                contract.UpdateTime = DateTime.Now;
                contract.UpdateUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RevokeApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.合約單;

            var contract = await GetContractByNoAsync(vm.TableId);
            if (contract == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (contract.Status != (int)ContractStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(contract.Status)}】，不可撤銷簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                // 是否為送簽人由引擎檢查，錯誤原樣轉出
                var result = await _approvalWorkflowService.RevokeApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                contract.Status = (int)ContractStatus.已退回;
                contract.UpdateTime = DateTime.Now;
                contract.UpdateUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> VoidApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.合約單;

            var contract = await GetContractByNoAsync(vm.TableId);
            if (contract == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (contract.Status != (int)ContractStatus.已核准)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(contract.Status)}】，只有已核准的合約單可以作廢");
            }

            return await RunInTransactionAsync(async () =>
            {
                // 是否為最後一關簽核人由引擎檢查（指定人員與指定角色兩種簽核模式都適用），錯誤原樣轉出
                var result = await _approvalWorkflowService.VoidApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 合約單沒有庫存要沖回，作廢只是終止單據
                contract.Status = (int)ContractStatus.已作廢;
                contract.UpdateTime = DateTime.Now;
                contract.UpdateUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        /// <summary>
        /// 以單號取得合約單（追蹤中）。簽核引擎的 TableId 為單號。
        /// </summary>
        private Task<t_3050Contract?> GetContractByNoAsync(string tableId)
        {
            return _db.t_3050Contract
                .Where(x => x.No == tableId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 在單一資料庫交易內執行動作：回傳失敗時整個回滾。
        /// 簽核引擎與本服務共用同一個 DbContext，所以引擎的多次儲存也會被這個交易包住。
        /// </summary>
        private async Task<ResultModel<string>> RunInTransactionAsync(Func<Task<ResultModel<string>>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var result = await action();
            if (!result)
            {
                await transaction.RollbackAsync();
                // 回滾後追蹤中的實體已與資料庫不一致，清除避免後續誤用
                _db.ChangeTracker.Clear();
                return result;
            }

            await transaction.CommitAsync();
            return result;
        }

        #endregion

        private static string GetStatusDisplay(int status)
        {
            return ((ContractStatus)status).GetDisplayName();
        }
    }
}
