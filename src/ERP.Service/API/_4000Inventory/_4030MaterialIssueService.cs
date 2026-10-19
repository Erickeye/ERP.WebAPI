using ClosedXML.Excel;
using ERP.Approval.Abstractions;
using ERP.Approval.Services;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    /// <summary>
    /// 領料單-服務
    /// </summary>
    public interface I_4030MaterialIssueService
    {
        /// <summary>
        /// 查詢領料單列表
        /// </summary>
        Task<ResultModel<PagedResult<MaterialIssueVM>>> Index(MaterialIssueSearchVM vm);

        /// <summary>
        /// 取得單張領料單
        /// </summary>
        Task<ResultModel<MaterialIssueVM>> Get(int id);

        /// <summary>
        /// 新增領料單（草稿）
        /// </summary>
        Task<ResultModel<MaterialIssueSaveResultVM>> Add(MaterialIssueAddVM vm);

        /// <summary>
        /// 修改領料單（僅草稿、已退回）
        /// </summary>
        Task<ResultModel<MaterialIssueSaveResultVM>> Edit(MaterialIssueAddVM vm);

        /// <summary>
        /// 刪除領料單（僅草稿）
        /// </summary>
        Task<ResultModel<string>> Delete(int id);

        /// <summary>
        /// 匯出領料單列表
        /// </summary>
        Task<ResultModel<FileModel>> Export(MaterialIssueSearchVM vm);

        /// <summary>
        /// 送出簽核
        /// </summary>
        Task<ResultModel<string>> SendApproval(ApprovalVM vm);

        /// <summary>
        /// 簽核（最後一關核准時扣庫存）
        /// </summary>
        Task<ResultModel<string>> Approval(ApprovalVM vm);

        /// <summary>
        /// 拒絕簽核
        /// </summary>
        Task<ResultModel<string>> RejectApproval(ApprovalVM vm);

        /// <summary>
        /// 撤銷送簽
        /// </summary>
        Task<ResultModel<string>> RevokeApproval(ApprovalVM vm);

        /// <summary>
        /// 作廢已核准的領料單（回補庫存）
        /// </summary>
        Task<ResultModel<string>> VoidApproval(ApprovalVM vm);
    }

    public class _4030MaterialIssueService : I_4030MaterialIssueService
    {
        private const string PurposeConfigType = "MaterialIssuePurpose";
        private const string LocationConfigType = "InventoryLocation";
        private const string SerialPrefix = "IS";
        private const string ConcurrencyMessage = "庫存已被異動，請重新操作";

        private readonly ERPDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly ISerialService _serialService;
        private readonly IApprovalWorkflowService _approvalWorkflowService;
        private readonly IStockMovementRecorder _stockMovementRecorder;

        public _4030MaterialIssueService(
            ERPDbContext db,
            ICurrentUserService currentUserService,
            ISerialService serialService,
            IApprovalWorkflowService approvalWorkflowService,
            IStockMovementRecorder stockMovementRecorder)
        {
            _db = db;
            _currentUserService = currentUserService;
            _serialService = serialService;
            _approvalWorkflowService = approvalWorkflowService;
            _stockMovementRecorder = stockMovementRecorder;
        }

        #region 查詢與匯出

        public async Task<ResultModel<PagedResult<MaterialIssueVM>>> Index(MaterialIssueSearchVM vm)
        {
            // 預設最新的單據在前面
            IQueryable<MaterialIssueVM> query = QueryList(vm).OrderByDescending(x => x.Id);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<MaterialIssueVM>());
            }

            var pagedResult = await query.ToPagedResultAsync(vm);

            foreach (var item in pagedResult.Items)
            {
                item.StatusDisplay = GetStatusDisplay(item.Status);
            }

            return ResultModel.Ok(pagedResult);
        }

        public async Task<ResultModel<MaterialIssueVM>> Get(int id)
        {
            var data = await QueryList(new MaterialIssueSearchVM())
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            data.StatusDisplay = GetStatusDisplay(data.Status);
            return ResultModel.Ok(data);
        }

        public async Task<ResultModel<FileModel>> Export(MaterialIssueSearchVM vm)
        {
            // 與列表共用同一個 EF 查詢，不使用預存程序，避免查詢條件與畫面不一致
            var list = await QueryList(vm)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var exportItems = list.Select(x => new MaterialIssueExportItemVM
            {
                No = x.No,
                IssueDate = x.IssueDate.ToString("yyyy/MM/dd"),
                PurposeName = x.PurposeName,
                LocationName = x.LocationName,
                ReceiverName = x.ReceiverName,
                CustomerName = x.CustomerName,
                ProjectName = x.ProjectName,
                StatusDisplay = GetStatusDisplay(x.Status),
                Note = x.Note
            }).ToList();

            // PDF
            if (vm.SaveFormat == 13)
            {
                return ResultModel.Ok(exportItems.ConvertToPdfFileModel("領料單列表"));
            }

            // XLSX（共用查詢模板）
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "共用查詢模板.xlsx");

            var workbook = new XLWorkbook(templatePath);
            workbook.BindQueryTable(exportItems, "領料單列表");

            var file = workbook.ConvertToFileModel((ClosedXmlSaveFormat)vm.SaveFormat, "領料單列表");

            return ResultModel.Ok(file);
        }

        private IQueryable<MaterialIssueVM> QueryList(MaterialIssueSearchVM vm)
        {
            var filter = SearchExpressionBuilder.Build<t_4030MaterialIssue>(vm);

            return _db.t_4030MaterialIssue
                .AsNoTracking()
                .Where(filter)
                .Select(x => new MaterialIssueVM
                {
                    Id = x.Id,
                    No = x.No,
                    IssueDate = x.IssueDate,
                    PurposeId = x.PurposeId,
                    PurposeName = x.Purpose.Name,
                    LocationId = x.LocationId,
                    LocationName = x.Location.Name,
                    ReceiverStaffId = x.ReceiverStaffId,
                    ReceiverName = x.ReceiverStaff.ChineseName,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer!.Name,
                    ProjectName = x.ProjectName,
                    Note = x.Note,
                    Status = x.Status,
                    CreateTime = x.CreateTime,
                    CreateUserId = x.CreateUserId,
                    CreateUserName = x.CreateUser.Name,
                    Items = x.t_4031MaterialIssueDetail.Select(d => new MaterialIssueItemVM
                    {
                        Id = d.Id,
                        InventoryId = d.InventoryId,
                        SupplierName = d.Inventory.Supplier.Name,
                        No = d.No,
                        Name = d.Name,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        InventoryQuantity = d.Inventory.Quantity
                    }).ToList()
                });
        }

        #endregion

        #region 新增、修改、刪除

        public async Task<ResultModel<MaterialIssueSaveResultVM>> Add(MaterialIssueAddVM vm)
        {
            var entity = new t_4030MaterialIssue
            {
                CreateTime = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                Status = (int)MaterialIssueStatus.草稿
            };

            var warnings = new List<string>();
            var result = await ModifyAsync(vm, entity, warnings);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            entity.No = await _serialService.GenerateAsync(SerialPrefix);

            _db.t_4030MaterialIssue.Add(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(new MaterialIssueSaveResultVM
            {
                Id = entity.Id,
                No = entity.No,
                Warnings = warnings
            });
        }

        public async Task<ResultModel<MaterialIssueSaveResultVM>> Edit(MaterialIssueAddVM vm)
        {
            var entity = await _db.t_4030MaterialIssue
                .Include(x => x.t_4031MaterialIssueDetail)
                .Where(x => x.Id == vm.Id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿與已退回可以編輯，簽核中、已核准、已作廢的內容不可再變動
            if (entity.Status != (int)MaterialIssueStatus.草稿 &&
                entity.Status != (int)MaterialIssueStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，不可編輯領料單");
            }

            var warnings = new List<string>();
            var result = await ModifyAsync(vm, entity, warnings);
            if (!result)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            await _db.SaveChangesAsync();

            return ResultModel.Ok(new MaterialIssueSaveResultVM
            {
                Id = entity.Id,
                No = entity.No,
                Warnings = warnings
            });
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await _db.t_4030MaterialIssue
                .Include(x => x.t_4031MaterialIssueDetail)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            // 只有草稿可以刪除；已退回保留紀錄
            if (entity.Status != (int)MaterialIssueStatus.草稿)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(entity.Status)}】，只有草稿可以刪除");
            }

            // 明細對庫存、單頭皆不串聯刪除，需先明確刪除明細
            _db.t_4031MaterialIssueDetail.RemoveRange(entity.t_4031MaterialIssueDetail);
            _db.t_4030MaterialIssue.Remove(entity);
            await _db.SaveChangesAsync();

            return ResultModel.Ok(entity.No);
        }

        /// <summary>
        /// 新增與修改共用：驗證輸入並設定單頭與明細。
        /// 庫存不足在草稿階段只產生警告，不阻擋儲存。
        /// </summary>
        private async Task<ResultModel<string>> ModifyAsync(
            MaterialIssueAddVM vm,
            t_4030MaterialIssue entity,
            List<string> warnings)
        {
            if (vm.Items == null || vm.Items.Count == 0)
            {
                return ResultModel.Error(ErrorCodeType.IncompleteInfo, "領料單至少需要一筆領料明細");
            }

            if (vm.Items.Any(x => x.Quantity <= 0))
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "領料數量必須大於 0");
            }

            var hasDuplicate = vm.Items
                .GroupBy(x => x.InventoryId)
                .Any(g => g.Count() > 1);
            if (hasDuplicate)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "同一張領料單內，庫存項目不可重複，請合併成一行");
            }

            var purposeExists = await _db.SystemConfig.AnyAsync(x =>
                x.Id == vm.PurposeId && x.ConfigType == PurposeConfigType && x.IsActive);
            if (!purposeExists)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "領料用途不存在或已停用");
            }

            var locationExists = await _db.SystemConfig.AnyAsync(x =>
                x.Id == vm.LocationId && x.ConfigType == LocationConfigType && x.IsActive);
            if (!locationExists)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "庫位不存在或已停用");
            }

            var receiverExists = await _db.t_1000Staff.AnyAsync(x =>
                x.StaffId == vm.ReceiverStaffId && !x.ResignationDate.HasValue);
            if (!receiverExists)
            {
                return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "領料人必須是在職員工");
            }

            if (vm.CustomerId.HasValue)
            {
                var customerExists = await _db.t_2000Customer.AnyAsync(x => x.Id == vm.CustomerId.Value);
                if (!customerExists)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "找不到客戶");
                }
            }

            var inventoryIds = vm.Items.Select(x => x.InventoryId).ToList();
            var inventories = await _db.t_4000Inventory
                .Where(x => inventoryIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            foreach (var item in vm.Items)
            {
                if (!inventories.TryGetValue(item.InventoryId, out var inventory))
                {
                    return ResultModel.Error(ErrorCodeType.NotFoundData, $"找不到庫存項目（識別碼 {item.InventoryId}）");
                }

                // 一單一庫位：明細的庫存項目必須屬於單頭選擇的庫位
                if (inventory.LocationId != vm.LocationId)
                {
                    return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid,
                        $"庫存項目【{inventory.No}】{inventory.Name} 不在所選庫位");
                }
            }

            entity.IssueDate = vm.IssueDate;
            entity.PurposeId = vm.PurposeId;
            entity.LocationId = vm.LocationId;
            entity.ReceiverStaffId = vm.ReceiverStaffId;
            entity.CustomerId = vm.CustomerId;
            entity.ProjectName = string.IsNullOrWhiteSpace(vm.ProjectName) ? null : vm.ProjectName;
            entity.Note = vm.Note;

            //---處理明細---

            //刪除不在輸入內的明細
            var deleteItems = entity.t_4031MaterialIssueDetail
                .Where(x => !inventoryIds.Contains(x.InventoryId))
                .ToList();
            _db.t_4031MaterialIssueDetail.RemoveRange(deleteItems);

            foreach (var item in vm.Items)
            {
                var inventory = inventories[item.InventoryId];

                var detail = entity.t_4031MaterialIssueDetail
                    .FirstOrDefault(x => x.InventoryId == item.InventoryId);

                if (detail == null)
                {
                    detail = new t_4031MaterialIssueDetail { InventoryId = item.InventoryId };
                    entity.t_4031MaterialIssueDetail.Add(detail);
                }

                detail.Quantity = item.Quantity;
                // 快照料號、品名、單位，單據歷史不隨庫存資料變動
                detail.No = inventory.No;
                detail.Name = inventory.Name;
                detail.Unit = inventory.Unit;

                var stock = inventory.Quantity ?? 0m;
                if (item.Quantity > stock)
                {
                    warnings.Add($"庫存不足：【{inventory.No}】{inventory.Name} 需求數量 {item.Quantity}，庫存數量 {stock}");
                }
            }

            return ResultModel.Ok();
        }

        #endregion

        #region 簽核動作（與扣庫存同一交易，見 ADR 0001）

        public async Task<ResultModel<string>> SendApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.領料單;

            var issue = await GetIssueForApprovalAsync(vm.TableId);
            if (issue == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (issue.Status != (int)MaterialIssueStatus.草稿 &&
                issue.Status != (int)MaterialIssueStatus.已退回)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(issue.Status)}】，不可送出簽核");
            }

            // 送簽前先檢查庫存，不足就拒絕送簽並列出品項（不鎖定庫存）
            var inventories = await LoadInventoriesAsync(issue);
            var shortage = FindShortage(issue, inventories);
            if (shortage != null)
            {
                return ResultModel.Error(ErrorCodeType.InsufficientInventory, shortage);
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.SendApprovalProcess(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                issue.Status = (int)MaterialIssueStatus.簽核中;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> Approval(ApprovalVM vm)
        {
            vm.TableType = TableType.領料單;

            var issue = await GetIssueForApprovalAsync(vm.TableId);
            if (issue == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (issue.Status != (int)MaterialIssueStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(issue.Status)}】，不可簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.Approval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 非最後一關：只推進簽核流程，不動庫存
                if (result.Data != ApprovalWorkflowService.CompletedMessage)
                {
                    return ResultModel.Ok($"{result.Data}");
                }

                //簽核全數完成 => 再檢查一次庫存，通過才扣庫存
                var inventories = await LoadInventoriesAsync(issue);
                var shortage = FindShortage(issue, inventories);
                if (shortage != null)
                {
                    // 回傳錯誤時外層交易會回滾，連同引擎剛寫入的核准紀錄一起還原
                    return ResultModel.Error(ErrorCodeType.InsufficientInventory, shortage);
                }

                var now = DateTime.Now;
                foreach (var item in issue.t_4031MaterialIssueDetail)
                {
                    _stockMovementRecorder.Apply(
                        inventories[item.InventoryId],
                        StockMovementType.領料出庫,
                        TableType.領料單,
                        issue.No,
                        -item.Quantity,
                        _currentUserService.UserId,
                        now);
                }

                issue.Status = (int)MaterialIssueStatus.已核准;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RejectApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.領料單;

            var issue = await GetIssueForApprovalAsync(vm.TableId);
            if (issue == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (issue.Status != (int)MaterialIssueStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(issue.Status)}】，不可拒絕簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.RejectApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                issue.Status = (int)MaterialIssueStatus.已退回;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> RevokeApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.領料單;

            var issue = await GetIssueForApprovalAsync(vm.TableId);
            if (issue == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (issue.Status != (int)MaterialIssueStatus.簽核中)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(issue.Status)}】，不可撤銷簽核");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.RevokeApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                issue.Status = (int)MaterialIssueStatus.已退回;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        public async Task<ResultModel<string>> VoidApproval(ApprovalVM vm)
        {
            vm.TableType = TableType.領料單;

            var issue = await GetIssueForApprovalAsync(vm.TableId);
            if (issue == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            if (issue.Status != (int)MaterialIssueStatus.已核准)
            {
                return ResultModel.Error(ErrorCodeType.InvalidDocumentStatus,
                    $"目前狀態為【{GetStatusDisplay(issue.Status)}】，只有已核准的領料單可以作廢");
            }

            return await RunInTransactionAsync(async () =>
            {
                var result = await _approvalWorkflowService.VoidApproval(vm);
                if (!result)
                {
                    return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
                }

                // 作廢 => 回補庫存。回補是加數量，不需要不足檢查
                var inventories = await LoadInventoriesAsync(issue);
                var now = DateTime.Now;
                foreach (var item in issue.t_4031MaterialIssueDetail)
                {
                    if (!inventories.TryGetValue(item.InventoryId, out var inventory))
                    {
                        return ResultModel.Error(ErrorCodeType.NotFoundData,
                            $"作廢失敗，找不到庫存項目：【{item.No}】{item.Name}");
                    }

                    _stockMovementRecorder.Apply(
                        inventory,
                        StockMovementType.領料作廢,
                        TableType.領料單,
                        issue.No,
                        item.Quantity,
                        _currentUserService.UserId,
                        now);
                }

                issue.Status = (int)MaterialIssueStatus.已作廢;
                await _db.SaveChangesAsync();

                return ResultModel.Ok($"{result.Data}");
            });
        }

        /// <summary>
        /// 以單號取得領料單（含明細，追蹤中），簽核引擎的 TableId 為單號
        /// </summary>
        private Task<t_4030MaterialIssue?> GetIssueForApprovalAsync(string tableId)
        {
            return _db.t_4030MaterialIssue
                .Include(x => x.t_4031MaterialIssueDetail)
                .Where(x => x.No == tableId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 載入領料單明細對應的庫存項目（追蹤中）
        /// </summary>
        private Task<Dictionary<int, t_4000Inventory>> LoadInventoriesAsync(t_4030MaterialIssue issue)
        {
            var inventoryIds = issue.t_4031MaterialIssueDetail
                .Select(x => x.InventoryId)
                .ToList();

            return _db.t_4000Inventory
                .Where(x => inventoryIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);
        }

        /// <summary>
        /// 找出庫存不足的明細，沒有不足回傳 null
        /// </summary>
        private static string? FindShortage(
            t_4030MaterialIssue issue,
            IReadOnlyDictionary<int, t_4000Inventory> inventories)
        {
            var lines = new List<string>();

            foreach (var item in issue.t_4031MaterialIssueDetail)
            {
                var stock = 0m;
                if (inventories.TryGetValue(item.InventoryId, out var inventory))
                {
                    stock = inventory.Quantity ?? 0m;
                }

                if (item.Quantity > stock)
                {
                    lines.Add($"【{item.No}】{item.Name} 需求數量 {item.Quantity}，庫存數量 {stock}");
                }
            }

            return lines.Count == 0 ? null : "庫存不足：" + string.Join("；", lines);
        }

        /// <summary>
        /// 在單一資料庫交易內執行動作：回傳失敗或發生並發衝突時整個回滾。
        /// 簽核引擎與本服務共用同一個 DbContext，所以引擎的多次儲存也會被這個交易包住。
        /// </summary>
        private async Task<ResultModel<string>> RunInTransactionAsync(Func<Task<ResultModel<string>>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
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
            catch (DbUpdateConcurrencyException)
            {
                // 庫存列在讀取後被別的單據異動（RowVersion 不符）
                await transaction.RollbackAsync();
                _db.ChangeTracker.Clear();
                return ResultModel.Error(ErrorCodeType.SpecificRecordChanged, ConcurrencyMessage);
            }
        }

        #endregion

        private static string GetStatusDisplay(int status)
        {
            return ((MaterialIssueStatus)status).GetDisplayName();
        }
    }
}
