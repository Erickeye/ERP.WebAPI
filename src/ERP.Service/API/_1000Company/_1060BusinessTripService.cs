using ERP.Approval.Abstractions;
using ERP.Approval.Services;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.Enums.Login;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._1000Company
{
    /// <summary>
    /// 出差單-服務
    /// </summary>
    public interface I_1060BusinessTripService
    {
        /// <summary>
        /// 查詢出差單列表（看得到自己建立、自己出差或待自己簽核的出差單，有「出差單管理」權限者看全部；
        /// 可依單號關鍵字、出差人、出差單狀態、日期區間（與出差起訖有交集）、「待我簽核」篩選，篩選只縮小不擴大可見範圍；
        /// 每筆帶 CanApprove、CanEdit、CanRevoke、CanVoid）
        /// </summary>
        Task<ResultModel<PagedResult<BusinessTripVM>>> Index(BusinessTripSearchVM vm);

        /// <summary>
        /// 取得單張出差單（簽核人與有「出差單管理」權限者也可檢視；由後端回傳目前使用者能做的動作 CanApprove、CanEdit、CanRevoke、CanVoid）
        /// </summary>
        Task<ResultModel<BusinessTripVM>> Get(int id);

        /// <summary>
        /// 新增出差單（草稿）
        /// </summary>
        Task<ResultModel<BusinessTripSaveResultVM>> Add(BusinessTripAddVM vm);

        /// <summary>
        /// 修改出差單（僅草稿、已退回）
        /// </summary>
        Task<ResultModel<BusinessTripSaveResultVM>> Edit(BusinessTripAddVM vm);

        /// <summary>
        /// 刪除出差單（僅草稿）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);

        /// <summary>
        /// 取得目前使用者對應的員工（供前端預設出差人）
        /// </summary>
        Task<ResultModel<SelectModel>> GetCurrentStaff();

        /// <summary>
        /// 送出簽核（草稿、已退回；送簽前檢查出差人日期重疊）
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
        /// 撤銷（只有簽核中；限送簽人，且為建單人或出差人；成功後單據變成已退回）
        /// </summary>
        Task<ResultModel<string>> RevokeApproval(ApprovalVM vm);

        /// <summary>
        /// 作廢（只有已核准；限最後一關簽核人；成功後單據變成已作廢，為終態）
        /// </summary>
        Task<ResultModel<string>> VoidApproval(ApprovalVM vm);
    }

    public class _1060BusinessTripService : I_1060BusinessTripService
    {
        private const string SerialPrefix = "BT";

        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISerialService _serialService;
        private readonly IApprovalWorkflowService _approvalWorkflowService;

        public _1060BusinessTripService(
            ERPDbContext db,
            ICurrentUserService currentUserService,
            ISerialService serialService,
            IApprovalWorkflowService approvalWorkflowService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
            _approvalWorkflowService = approvalWorkflowService;
        }

        #region 查詢

        public async Task<ResultModel<PagedResult<BusinessTripVM>>> Index(BusinessTripSearchVM vm)
        {
            if (vm.PeriodStart.HasValue && vm.PeriodEnd.HasValue &&
                vm.PeriodEnd.Value.Date < vm.PeriodStart.Value.Date)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差期間結束不得早於開始");
            }

            var pending = await GetMyPendingTripNosAsync();
            if (!pending)
            {
                return ResultModel.Error(pending.ErrorCode, pending.ErrorMessage);
            }
            var pendingNos = pending.Data!;

            var trips = await ViewableTripsForReadingAsync(pendingNos);
            if (vm.PendingMyApproval)
            {
                // 「待我簽核」取自引擎的待簽核查詢，並限定簽核中的出差單
                trips = trips.Where(x => x.Status == (int)BusinessTripStatus.簽核中 && pendingNos.Contains(x.No));
            }

            // 篩選條件只在可見範圍內再縮小，不會擴大可見範圍
            trips = trips.Where(SearchExpressionBuilder.Build<t_1060BusinessTrip>(vm));

            // 日期區間以「與出差起訖有交集」為準：出差迄日不早於區間開始，且出差起日不晚於區間結束（邊界日算有交集）
            if (vm.PeriodStart.HasValue)
            {
                var periodStart = vm.PeriodStart.Value.Date;
                trips = trips.Where(x => x.EndDate >= periodStart);
            }
            if (vm.PeriodEnd.HasValue)
            {
                var periodEnd = vm.PeriodEnd.Value.Date;
                trips = trips.Where(x => x.StartDate <= periodEnd);
            }

            // 預設依出差起日由新到舊，起日相同時新建立的在前
            IQueryable<BusinessTripVM> query = QueryList(trips)
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.Id);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                // 出差天數、狀態顯示與是否輪到我簽核是輸出時才計算的欄位，不是資料庫欄位，不開放排序
                var sortColumns = SortHelper.GetColumns<BusinessTripVM>();
                sortColumns.Remove(nameof(BusinessTripVM.Days));
                sortColumns.Remove(nameof(BusinessTripVM.StatusDisplay));
                sortColumns.Remove(nameof(BusinessTripVM.CanApprove));
                sortColumns.Remove(nameof(BusinessTripVM.CanEdit));
                sortColumns.Remove(nameof(BusinessTripVM.CanRevoke));
                sortColumns.Remove(nameof(BusinessTripVM.CanVoid));
                query = query.ApplySort(vm, sortColumns);
            }

            var pagedResult = await query.ToPagedResultAsync(vm);

            await FillComputedFieldsAsync(pagedResult.Items, pendingNos);

            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<BusinessTripVM>> Get(int id)
        {
            var pending = await GetMyPendingTripNosAsync();
            if (!pending)
            {
                return ResultModel.Error(pending.ErrorCode, pending.ErrorMessage);
            }

            var data = await QueryList(await ViewableTripsForReadingAsync(pending.Data!))
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            await FillComputedFieldsAsync(new List<BusinessTripVM> { data }, pending.Data!);
            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<SelectModel>> GetCurrentStaff()
        {
            // 出差人必須是在職員工，所以離職者不提供預設值
            var staff = await CurrentStaffQuery()
                .Where(x => !x.ResignationDate.HasValue)
                .Select(x => new SelectModel
                {
                    Value = x.StaffId,
                    Text = x.ChineseName
                })
                .FirstOrDefaultAsync();

            if (staff == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到目前使用者對應的在職員工");
            }

            return ResultModel.Ok(staff);
        }

        private IQueryable<BusinessTripVM> QueryList(IQueryable<t_1060BusinessTrip> trips)
        {
            return trips
                .AsNoTracking()
                .Select(x => new BusinessTripVM
                {
                    Id = x.Id,
                    No = x.No,
                    TravelerStaffId = x.TravelerStaffId,
                    TravelerName = x.TravelerStaff.ChineseName,
                    Destination = x.Destination,
                    Reason = x.Reason,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer!.Name,
                    Note = x.Note,
                    Status = x.Status,
                    CreateTime = x.CreateTime,
                    CreateUserId = x.CreateUserId,
                    CreateUserName = x.CreateUser.Name
                });
        }

        /// <summary>
        /// 列表與檢視的可見範圍：有「出差單管理」權限者看全部，其餘為 ViewableTrips。
        /// 管理權限只放寬列表與檢視；編輯、送簽、撤銷、核准、拒絕、作廢仍各自依 OwnTrips 或簽核人範圍，不看管理權限。
        /// </summary>
        private async Task<IQueryable<t_1060BusinessTrip>> ViewableTripsForReadingAsync(List<string> pendingNos)
        {
            return await HasManagePermissionAsync()
                ? _db.t_1060BusinessTrip
                : ViewableTrips(pendingNos);
        }

        /// <summary>
        /// 目前使用者的角色是否有「出差單管理」權限（與登入時寫入 JWT 的來源相同：Permission 表的角色與頁面識別碼）
        /// </summary>
        private Task<bool> HasManagePermissionAsync()
        {
            var roleId = _currentUserService.RoleId;
            var pageId = (int)PermissionType.出差單管理;

            return _db.Permission.AnyAsync(x => x.RoleId == roleId && x.PageId == pageId);
        }

        /// <summary>
        /// 目前使用者以建單人、出差人或簽核人身分「檢視」得到的出差單（不含管理權限）：自己的（見 OwnTrips），加上簽核人身分的出差單——
        /// 輪到自己簽核的，或簽核紀錄中有自己（指定人員）／自己的角色正在待簽（指定角色）的。
        /// 簽核人不必是建單人或出差人；編輯、刪除、送簽仍只限 OwnTrips。簽核動作（核准、拒絕、作廢）找單也用這個範圍，不放寬給管理權限。
        /// </summary>
        private IQueryable<t_1060BusinessTrip> ViewableTrips(List<string> pendingNos)
        {
            var userId = _currentUserService.UserId;
            var roleId = _currentUserService.RoleId;
            var tableType = (int)TableType.出差單;
            var ownTripIds = OwnTrips().Select(x => x.Id);

            // 送簽時引擎會建立一筆 StepOrder 為 0 的紀錄並帶送簽人的角色，不能拿它當簽核人依據，否則同角色的人都看得到
            var signingRecords = _db.ApprovalRecord
                .Where(r => r.TableType == tableType &&
                            r.StepOrder >= 1 &&
                            (r.UserId == userId ||
                             (r.RoleId == roleId && r.Status == (int)ApprovalStatus.Pending)));

            return _db.t_1060BusinessTrip
                .Where(x => ownTripIds.Contains(x.Id) ||
                            pendingNos.Contains(x.No) ||
                            signingRecords.Any(r => r.TableId == x.No));
        }

        /// <summary>
        /// 取得引擎「待簽核」查詢中，輪到目前使用者簽的出差單單號
        /// </summary>
        private async Task<ResultModel<List<string>>> GetMyPendingTripNosAsync()
        {
            var notify = await _approvalWorkflowService.GetApprovalNotify();
            if (!notify)
            {
                return ResultModel.Error(notify.ErrorCode, notify.ErrorMessage);
            }

            var nos = notify.Data!.Items
                .Where(x => x.TableType == (int)TableType.出差單 && x.TableId != null)
                .Select(x => x.TableId!)
                .Distinct()
                .ToList();
            return ResultModel<List<string>>.Ok(nos);
        }

        /// <summary>
        /// 填入輸出時才計算的欄位：狀態顯示，以及目前使用者能做的動作 CanApprove、CanEdit、CanRevoke、CanVoid
        /// （由後端判斷，前端不自行推算）
        /// </summary>
        private async Task FillComputedFieldsAsync(List<BusinessTripVM> items, List<string> pendingNos)
        {
            if (items.Count == 0)
            {
                return;
            }

            var userId = _currentUserService.UserId;

            // 編輯與撤銷限建單人或出差人（與 Edit、RevokeApproval 的範圍一致）
            var ids = items.Select(x => x.Id).ToList();
            var ownIds = (await OwnTrips()
                .Where(x => ids.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync()).ToHashSet();

            // 撤銷與作廢都看最近一輪的簽核紀錄
            var roundNos = items
                .Where(x => x.Status == (int)BusinessTripStatus.簽核中 || x.Status == (int)BusinessTripStatus.已核准)
                .Select(x => x.No!)
                .ToList();
            var rounds = await LoadLatestRoundsAsync(roundNos);

            foreach (var item in items)
            {
                var isOwn = ownIds.Contains(item.Id);
                rounds.TryGetValue(item.No!, out var round);

                item.StatusDisplay = GetStatusDisplay(item.Status);
                item.CanApprove = item.Status == (int)BusinessTripStatus.簽核中 && pendingNos.Contains(item.No!);
                item.CanEdit = isOwn &&
                               (item.Status == (int)BusinessTripStatus.草稿 || item.Status == (int)BusinessTripStatus.已退回);
                item.CanRevoke = isOwn &&
                                 item.Status == (int)BusinessTripStatus.簽核中 &&
                                 round?.SenderUserId == userId;
                item.CanVoid = item.Status == (int)BusinessTripStatus.已核准 &&
                               round != null &&
                               round.LastStepUserIds.Contains(userId);
            }
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

            var tableType = (int)TableType.出差單;
            var records = await _db.ApprovalRecord
                .AsNoTracking()
                .Where(r => r.TableType == tableType && nos.Contains(r.TableId))
                .Select(r => new { r.Id, r.TableId, r.StepOrder, r.UserId, r.Status })
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

        /// <summary>
        /// 目前使用者自己的出差單：自己建立的，或出差人是自己的。
        /// 在 Service 以登入者強制限縮，不信任前端傳入的條件。
        /// </summary>
        private IQueryable<t_1060BusinessTrip> OwnTrips()
        {
            var userId = _currentUserService.UserId;
            var currentStaffIds = CurrentStaffQuery().Select(x => x.StaffId);

            return _db.t_1060BusinessTrip
                .Where(x => x.CreateUserId == userId || currentStaffIds.Contains(x.TravelerStaffId));
        }

        /// <summary>
        /// 目前使用者對應的員工：使用者帳號與員工的使用者帳號相同（員工資料的「使用者帳號」欄位）
        /// </summary>
        private IQueryable<t_1000Staff> CurrentStaffQuery()
        {
            var userId = _currentUserService.UserId;

            return _db.t_1000Staff
                .Where(s => _db.User.Any(u => u.Id == userId && u.Account == s.Account));
        }

        #endregion

        #region 新增、修改、刪除

        public async Task<ResultModel<BusinessTripSaveResultVM>> Add(BusinessTripAddVM vm)
        {
            var entity = new t_1060BusinessTrip
            {
                CreateTime = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                Status = (int)BusinessTripStatus.草稿
            };

            var result = await ModifyAsync(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            entity.No = await _serialService.GenerateAsync(SerialPrefix);

            _db.t_1060BusinessTrip.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(new BusinessTripSaveResultVM
            {
                Id = entity.Id,
                No = entity.No
            });
        }

        public async Task<ResultModel<BusinessTripSaveResultVM>> Edit(BusinessTripAddVM vm)
        {
            var entity = await OwnTrips()
                .Where(x => x.Id == vm.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿與已退回可以編輯，簽核中、已核准、已作廢的內容不可再變動
            if (entity.Status != (int)BusinessTripStatus.草稿 &&
                entity.Status != (int)BusinessTripStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，不可編輯出差單");
            }

            var result = await ModifyAsync(vm, entity);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            await _db.SaveChangesAsync();

            return ResultModel.Ok(new BusinessTripSaveResultVM
            {
                Id = entity.Id,
                No = entity.No
            });
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await OwnTrips()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿可以刪除；已退回保留紀錄
            if (entity.Status != (int)BusinessTripStatus.草稿)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，只有草稿可以刪除");
            }

            _db.t_1060BusinessTrip.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.No);
        }

        /// <summary>
        /// 新增與修改共用：驗證輸入並設定單頭欄位。
        /// </summary>
        private async Task<ResultModel<string>> ModifyAsync(BusinessTripAddVM vm, t_1060BusinessTrip entity)
        {
            // Controller 有 [ValidateModel] 擋必填；Service 仍自行檢查，避免空白字串或被其他呼叫端繞過
            var destination = vm.Destination?.Trim();
            if (string.IsNullOrEmpty(destination))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差地點必填");
            }

            var reason = vm.Reason?.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差事由必填");
            }

            if (!vm.StartDate.HasValue || !vm.EndDate.HasValue)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差起日與迄日必填");
            }

            // 起日不設下限（允許先出差、回來才補單），只檢查迄日不得早於起日
            if (vm.EndDate.Value.Date < vm.StartDate.Value.Date)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差迄日不得早於起日");
            }

            var travelerExists = await _db.t_1000Staff.AnyAsync(x =>
                x.StaffId == vm.TravelerStaffId && !x.ResignationDate.HasValue);
            if (!travelerExists)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "出差人必須是在職員工");
            }

            if (vm.CustomerId.HasValue)
            {
                var customerExists = await _db.t_2000Customer.AnyAsync(x => x.Id == vm.CustomerId.Value);
                if (!customerExists)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "找不到客戶");
                }
            }

            entity.TravelerStaffId = vm.TravelerStaffId;
            entity.Destination = destination;
            entity.Reason = reason;
            entity.StartDate = vm.StartDate.Value.Date;
            entity.EndDate = vm.EndDate.Value.Date;
            entity.CustomerId = vm.CustomerId;
            entity.Note = vm.Note;

            return ResultModel.Ok();
        }

        #endregion

        #region 簽核動作（與單據狀態同一交易，見差勤 ADR 0001）

        public async Task<ResultModel<string>> SendApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.出差單;

            var trip = await GetTripForApprovalAsync(vm.TableId);
            if (trip == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 草稿與已退回可送簽；已作廢為終態，不可重送
            if (trip.Status != (int)BusinessTripStatus.草稿 &&
                trip.Status != (int)BusinessTripStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(trip.Status)}】，不可送出簽核");
            }

            // 重疊檢查需看全部有效出差單，不受目前使用者的可見範圍限制
            var conflicts = await _db.t_1060BusinessTrip
                .AsNoTracking()
                .Where(x => x.Id != trip.Id &&
                            x.TravelerStaffId == trip.TravelerStaffId &&
                            (x.Status == (int)BusinessTripStatus.簽核中 || x.Status == (int)BusinessTripStatus.已核准) &&
                            x.StartDate <= trip.EndDate &&
                            x.EndDate >= trip.StartDate)
                .OrderBy(x => x.StartDate)
                .ThenBy(x => x.Id)
                .Select(x => new { x.No, x.StartDate, x.EndDate })
                .ToListAsync();
            if (conflicts.Count > 0)
            {
                var lines = conflicts.Select(x => $"{x.No}（{x.StartDate:yyyy/MM/dd}～{x.EndDate:yyyy/MM/dd}）");
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid,
                    $"出差人在該期間已有出差單：{string.Join("、", lines)}");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.SendApprovalProcess(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                trip.Status = (int)BusinessTripStatus.簽核中;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        /// <summary>
        /// 以單號取得出差單（追蹤中，限制在可見範圍），簽核引擎的 TableId 為單號
        /// </summary>
        private Task<t_1060BusinessTrip?> GetTripForApprovalAsync(string tableId)
        {
            return OwnTrips()
                .Where(x => x.No == tableId)
                .FirstOrDefaultAsync();
        }

        public async Task<ResultModel<string>> Approval(ApprovalVM vm)
        {
            vm.TableType = TableType.出差單;

            var found = await GetTripForSigningAsync(vm.TableId);
            if (!found)
            {
                return ResultModel.Error(found.ErrorCode, found.ErrorMessage);
            }
            var trip = found.Data!;

            if (trip.Status != (int)BusinessTripStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(trip.Status)}】，不可簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.Approval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 以引擎回報流程完成為準（比照領料單）；中間關卡或同關尚有人未核准時維持簽核中
                if (result.Data == ApprovalWorkflowService.CompletedMessage)
                {
                    trip.Status = (int)BusinessTripStatus.已核准;
                    await _db.SaveChangesAsync();
                }

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RejectApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.出差單;

            var found = await GetTripForSigningAsync(vm.TableId);
            if (!found)
            {
                return ResultModel.Error(found.ErrorCode, found.ErrorMessage);
            }
            var trip = found.Data!;

            if (trip.Status != (int)BusinessTripStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(trip.Status)}】，不可拒絕簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                // 拒絕原因必填由引擎檢查，錯誤原樣轉出
                var result = await _approvalWorkflowService.RejectApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                trip.Status = (int)BusinessTripStatus.已退回;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RevokeApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.出差單;

            // 撤銷限建單人或出差人範圍；是否為送簽人由引擎檢查，錯誤原樣轉出
            var trip = await GetTripForApprovalAsync(vm.TableId);
            if (trip == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (trip.Status != (int)BusinessTripStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(trip.Status)}】，不可撤銷簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.RevokeApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                trip.Status = (int)BusinessTripStatus.已退回;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> VoidApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.出差單;

            // 最後一關簽核人不必是建單人或出差人，所以用簽核人的檢視範圍找單；是否為最後一關簽核人由引擎檢查，錯誤原樣轉出
            var found = await GetTripForSigningAsync(vm.TableId);
            if (!found)
            {
                return ResultModel.Error(found.ErrorCode, found.ErrorMessage);
            }
            var trip = found.Data!;

            if (trip.Status != (int)BusinessTripStatus.已核准)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(trip.Status)}】，只有已核准的出差單可以作廢");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.VoidApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 出差單沒有庫存要沖回，作廢只是終止單據
                trip.Status = (int)BusinessTripStatus.已作廢;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        /// <summary>
        /// 以單號取得簽核人可檢視的出差單（追蹤中）。簽核人不必是建單人或出差人，
        /// 看不到的單據視為找不到；輪到誰簽、是否擋下由引擎判斷。
        /// </summary>
        private async Task<ResultModel<t_1060BusinessTrip>> GetTripForSigningAsync(string tableId)
        {
            var pending = await GetMyPendingTripNosAsync();
            if (!pending)
            {
                return ResultModel.Error(pending.ErrorCode, pending.ErrorMessage);
            }

            var trip = await ViewableTrips(pending.Data!)
                .Where(x => x.No == tableId)
                .FirstOrDefaultAsync();
            if (trip == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            return ResultModel.Ok(trip);
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
            return ((BusinessTripStatus)status).GetDisplayName();
        }
    }
}
