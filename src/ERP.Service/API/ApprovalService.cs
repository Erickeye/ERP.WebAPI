using System.Data;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.ViewModels;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERP.Service.API
{
    public interface IApprovalService
    {
        Task<ResultModel<string>> SendApprovalProcess(SendApprovalProcessVM data);
        Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(SendApprovalProcessVM data);
        Task<ResultModel<string>> Approval(ApprovalVM data);
        Task<ResultModel<string>> RejectApproval(ApprovalVM data);
        Task<ResultModel<ListResult<ApprovalSettings>>> SettingsIndex();
        Task<ResultModel<ApprovalCheckSettingsVM>> CheckSettings(int approvalSettingsId);
        Task<ResultModel<string>> CreateSettings(ApprovalSettingsInputVM vm);
        Task<ResultModel<string>> EditSetting(ApprovalCheckSettingsVM vm);
        Task<ResultModel<string>> DeleteSettings(int id);
    }
    public class ApprovalService : IApprovalService
    {
        private readonly ERPDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ApprovalService(ERPDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        //============================= 【簽核動作執行】=============================
        public async Task<ResultModel<string>> SendApprovalProcess(SendApprovalProcessVM data)
        {
            //檢查設定檔
            var settingList = await _context.ApprovalSettings
                .Include(x => x.ApprovalStep)
                    .ThenInclude(x => x.ApprovalStepNumber)
                 .Include(x => x.ApprovalStep)
                    .ThenInclude(x => x.ApprovalRecord)
                .Where(x => x.TableType == (int)data.TableType &&
                       x.IsActive == true)
                .ToListAsync();
            if (settingList.Count == 0)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該簽核設定檔");
            }
            else if (settingList.Count > 1)
            {
                return ResultModel.Error(ErrorCodeType.MultipleApprovalSettingsExists);
            }
            var setting = settingList.FirstOrDefault()!;

            //檢查該單號是否已送過簽核流程
            //var existTableId = await _context.ApprovalRecord
            //    .AnyAsync(x => x.TableId == data.TableId &&
            //                   x.TableType == setting.TableType &&
            //                   (x.Status == (int)ApprovalStatus.Pending));
            //if (existTableId)
            //{
            //    return ResultModel.Error(ErrorCodeType.ApprovalExists);
            //}

            var lastRecord = await _context.ApprovalRecord
                .Where(x =>
                    x.TableId == data.TableId &&
                    x.TableType == setting.TableType
                )
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            //如果已經有流程再進行(不是遭拒絕)

            if (lastRecord != null &&
                (lastRecord.Status == (int)ApprovalStatus.Pending || lastRecord.Status == (int)ApprovalStatus.Approved))
            {
                return ResultModel.Error(ErrorCodeType.ApprovalExists);
            }

            var tableCheckers = new Dictionary<TableType, Func<string, Task<bool>>>
            {
                { TableType.請假單, async id => await _context.t_1030Dayoff.AnyAsync(x => x.Id.ToString() == id) },
                { TableType.公文,   async id => await _context.t_1040Document.AnyAsync(x => x.Id.ToString() == id) }
            };
            //檢查是否有該單號
            if (tableCheckers.TryGetValue((TableType)setting.TableType, out var checker))
            {
                var exists = await checker(data.TableId);
                if (!exists)
                {
                    return ResultModel.Error(ErrorCodeType.NotFoundData);
                }
            }
            else
            {
                return ResultModel.Error(ErrorCodeType.InvalidApproval, "不支援的簽核資料類型");
            }

            int roleId = _currentUserService.RoleId;
            int userId = _currentUserService.UserId;
            //第一筆新增申請者-預設簽核狀態通過
            var fitstRecord = new ApprovalRecord
            {
                ApprovalStepId = 0, //等第一步Step再帶入
                TableId = data.TableId,
                StepOrder = 0,
                Status = (int)ApprovalStatus.Approved,
                RoleId = roleId,
                UserId = userId,
                Date = DateTime.Now,
                Memo = "申請者自動通過",
                TableType = (int)data.TableType
            };
            _context.Add(fitstRecord);

            var steps = setting.ApprovalStep.ToList();

            int stepCount = 0;
            //該簽核設定底下的每個流程
            foreach (var step in steps)
            {
                stepCount++;
                switch (step.Mode)
                {
                    case (int)ApprovalMode.Specify:
                        //指定人員
                        var approvalUser = step.ApprovalStepNumber.ToList();
                        foreach (var item in approvalUser)
                        {
                            var recordx = new ApprovalRecord
                            {
                                ApprovalStepId = step.Id,
                                //RoleId = step.RoleId,
                                TableId = data.TableId,
                                StepOrder = step.StepOrder,
                                Status = (int)ApprovalStatus.Pending,
                                UserId = item.UserId,
                                TableType = (int)data.TableType
                            };
                            //step整個簽核訊息先建立，但只顯示step1的訊息通知
                            if (stepCount == 1)
                            {
                                fitstRecord.ApprovalStepId = step.Id;
                            }
                            _context.Add(recordx);
                        }
                        break;
                    case (int)ApprovalMode.Role:
                        //指定角色(設定數量[RequiredCounts])
                        for (int i = 0; i < step.RequiredCounts; i++)
                        {
                            _context.Add(new ApprovalRecord
                            {
                                ApprovalStepId = step.Id,
                                RoleId = step.RoleId,
                                TableId = data.TableId,
                                StepOrder = step.StepOrder,
                                Status = (int)ApprovalStatus.Pending,
                                TableType = (int)data.TableType
                            });
                        }
                        break;
                }
            }
            await _context.SaveChangesAsync();
            return ResultModel.Ok("簽核流程成功送出");
        }
        public async Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(SendApprovalProcessVM data)
        {
            var recoreds = await _context.ApprovalRecord
                .Where(x => x.TableId == data.TableId && x.TableId == data.TableId)
                .ToListAsync();

            var userDict = await _context.User
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            var roleDict = await _context.Role
                .ToDictionaryAsync(x => x.Id, x => x.RoleName);

            if (!recoreds.Any())
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該【簽核紀錄】。");
            }

            var vm = recoreds.Select(x => new GetApprovalProgressVM
            {
                Id = x.Id,
                StepOrder = x.StepOrder,
                TableType = x.TableType,
                TableName = x.TableType.GetDisplayName<TableType>(),
                TableId = x.TableId,
                UserId = x.UserId,
                UserName = x.UserId.HasValue && userDict.TryGetValue(x.UserId.Value, out var userName)
                    ? userName
                    : null,
                RoleId = x.RoleId,
                RoleName = x.UserId.HasValue && roleDict.TryGetValue(x.UserId.Value, out var roleName)
                    ? roleName
                    : null,
                Status = x.Status,
                StatusDisplay = x.Status.GetDisplayName<ApprovalStatus>(),
                Memo = x.Memo
            })
            .ToList();

            return ResultModel.Ok(vm);
        }
        public async Task<ResultModel<string>> Approval(ApprovalVM data)
        {
            int userId = _currentUserService.UserId;
            int roleId = _currentUserService.RoleId;

            var records = await _context.ApprovalRecord
                .Where(x => x.TableType == (int)data.TableType &&
                    x.TableId == data.TableId &&
                    x.StepOrder >= 1 &&
                    x.Status == (int)ApprovalStatus.Pending)
                .ToListAsync();

            var record = records
                .Where(x => x.UserId == userId || x.RoleId == roleId)
                .FirstOrDefault();

            if (record == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該簽核內容");
            }
            // 檢查有無前面尚未完成的簽核   
            var pendingPrevious = records
                .Where(x => x.StepOrder < record.StepOrder)
                .Any();

            if (pendingPrevious)
            {
                return ResultModel.Error(ErrorCodeType.NotYetTurnForApprovalStep);
            }

            //簽核狀態檢查
            if (record.Status == (int)ApprovalStatus.Approved)
            {
                return ResultModel.Error(ErrorCodeType.IsAlreadyApproval);
            }
            //如果是角色模式 => 防止重複使用者簽核

            record.Date = DateTime.Now;
            record.Status = (int)ApprovalStatus.Approved;
            record.Memo = data.Memo;

            //該簽核還有其他使用者未簽核 x.StepOrder == record.StepOrder
            var isPendingCount = records
                .Where(x => x.StepOrder == record.StepOrder)
                .Count();
            //只剩一筆代表目前階段只剩下當前這筆Record
            if (isPendingCount > 1)
            {
                await _context.SaveChangesAsync();
                return ResultModel.Ok("簽核成功，該階段等待其他人員簽核");
            }
            //簽核階段全部完成(沒有下一個階段的Step階段)
            var nextStep = records
                .Where(x => x.StepOrder == record.StepOrder + 1);
            if (nextStep.Count() == 0)
            {
                await _context.SaveChangesAsync();
                return ResultModel.Ok("簽核成功，簽核作業已全數完成");
            }
            //該階段完成但還有下個階段
            //找出下個階段的通知訊息
            //var nextUsersNotification = _context.Notification
            //    .Where(x => x.TargetId == record.TableId && x.IsShow == false);
            //var nextUsers = nextStep.Select(x => x.UserId).ToList();

            //var notificationDict = nextUsersNotification.ToDictionary(x => x.UserId);

            //foreach (var user in nextUsers)
            //{
            //    if (!notificationDict.TryGetValue((int)user!, out var notification))
            //    {
            //        return ResultModel.Error(ErrorCodeType.UserNotFound);
            //    }
            //    //開啟下個階段的通知訊息
            //    notification.IsShow = true;
            //}
            await _context.SaveChangesAsync();
            return ResultModel.Ok("簽核成功，進到下個階段");
        }
        public async Task<ResultModel<string>> RejectApproval(ApprovalVM data)
        {
            int userId = _currentUserService.UserId;

            if (data.Memo == null || data.Memo == "")
            {
                return ResultModel.Error(ErrorCodeType.IncompleteInfo, "拒絕簽核需要訊息");
            }

            var record = await _context.ApprovalRecord
                .Where(x => x.TableType == (int)data.TableType &&
                        x.TableId == data.TableId &&
                        x.UserId == userId &&
                        x.StepOrder >= 1 &&
                        x.Status == (int)ApprovalStatus.Pending)
                .FirstOrDefaultAsync();
            if (record == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該簽核內容");
            }
            // 檢查有無前面尚未完成的簽核
            var pendingPrevious = await _context.ApprovalRecord
                .AnyAsync(x => x.TableType == (int)data.TableType &&
                    x.TableId == data.TableId &&
                    x.StepOrder >= 1 &&
                    x.StepOrder < record.StepOrder &&
                    x.Status == (int)ApprovalStatus.Pending);
            if (pendingPrevious)
            {
                return ResultModel.Error(ErrorCodeType.NotYetTurnForApprovalStep);
            }

            var recordList = await _context.ApprovalRecord
                .Where(x => x.TableType == (int)data.TableType &&
                            x.TableId == data.TableId &&
                            x.Status == (int)ApprovalStatus.Pending)
                .ToListAsync();

            foreach (var recordItem in recordList)
            {
                if (recordItem.Status == (int)ApprovalStatus.Pending)
                {
                    recordItem.Status = (int)ApprovalStatus.GetRejected;
                }
            }

            record.Date = DateTime.Now;
            record.Status = (int)ApprovalStatus.Rejected;
            record.Memo = data.Memo;

            await _context.SaveChangesAsync();
            return ResultModel.Ok("已拒絕該簽核作業");
        }

        //============================= 【1.簽核模組】=============================
        public async Task<ResultModel<ListResult<ApprovalSettings>>> SettingsIndex()
        {
            var result = new ResultModel<ListResult<ApprovalSettings>>();
            var list = await _context.ApprovalSettings.ToListAsync();
            return ResultModel.Ok(list);
        }
        public async Task<ResultModel<ApprovalCheckSettingsVM>> CheckSettings(int approvalSettingsId)
        {
            var setting = await _context.ApprovalSettings
                .Where(x => x.Id == approvalSettingsId)
                .Select(x => new ApprovalCheckSettingsVM
                {
                    Id = x.Id,
                    TableType = x.TableType,
                    TableTypeDisplay = x.TableType.GetDisplayName<TableType>(),
                    Name = x.Name,
                    IsActive = x.IsActive,
                    //【2.簽核步驟】
                    Steps = x.ApprovalStep
                    .Select(x => new ApprovalStepVM
                    {
                        Id = x.Id,
                        StepOrder = x.StepOrder,
                        RoleId = x.RoleId,
                        Mode = x.Mode,
                        ModeDisplay = x.Mode.GetDisplayName<ApprovalMode>(),
                        RequiredCounts = x.RequiredCounts,
                        // 【3.簽核步驟成員】
                        StepNumbers = x.ApprovalStepNumber
                        .Select(x => new ApprovalStepNumberVM
                        {
                            Id = x.Id,
                            ApprovalStepId = x.ApprovalStepId,
                            UserId = x.UserId
                        })
                        .ToList()
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync();


            if (setting == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            return ResultModel.Ok(setting);
        }
        public async Task<ResultModel<string>> CreateSettings(ApprovalSettingsInputVM vm)
        {
            var entity = new ApprovalSettings
            {
                TableType = vm.TableType,
                Name = vm.Name,
                IsActive = vm.IsActive
            };
            _context.Add(entity);

            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料成功新增");
        }
        public async Task<ResultModel<string>> EditSetting(ApprovalCheckSettingsVM vm)
        {
            var entitySetting = await _context.ApprovalSettings
                .Include(x => x.ApprovalStep)
                    .ThenInclude(x => x.ApprovalStepNumber)
                .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (entitySetting == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            // =============== 處理 Step ===============
            // 刪除 Step（VM 沒有的）
            var deleteSteps = entitySetting.ApprovalStep
                .Where(x => (!vm.Steps.Any(c => c.Id == x.Id)))
                .ToList();

            foreach (var step in deleteSteps)
            {
                // 先刪子層 StepNumber
                _context.RemoveRange(step.ApprovalStepNumber);
                _context.Remove(step);
            }

            foreach (var stepVm in vm.Steps)
            {
                var stepEntity = entitySetting.ApprovalStep.FirstOrDefault(x => x.Id == stepVm.Id);
                if (stepEntity == null)
                {
                    //新增步驟
                    stepEntity = new ApprovalStep
                    {
                        StepOrder = 0,
                    };
                    entitySetting.ApprovalStep.Add(stepEntity);
                }
                stepEntity.RoleId = stepVm.RoleId;
                stepEntity.Mode = stepVm.Mode;
                stepEntity.RequiredCounts = stepVm.RequiredCounts;


                // =============== 處理 StepNumber ===============

                // 3-1 刪除 StepNumber（VM 沒有的）
                var deleteNumbers = stepEntity.ApprovalStepNumber
                    .Where(x => !stepVm.StepNumbers.Any(n => n.Id == x.Id))
                    .ToList();

                foreach (var num in deleteNumbers)
                {
                    _context.Remove(num);
                }

                // 3-2 新增 / 更新 StepNumber
                foreach (var numVm in stepVm.StepNumbers)
                {
                    var numEntity = stepEntity.ApprovalStepNumber
                        .FirstOrDefault(x => x.Id == numVm.Id);

                    // 新增
                    if (numEntity == null)
                    {
                        numEntity = new ApprovalStepNumber();
                        stepEntity.ApprovalStepNumber.Add(numEntity);
                    }
                    // 更新
                    numEntity.UserId = numVm.UserId;
                }
            }

            await _context.SaveChangesAsync();
            return ResultModel.Ok();
        }        
        public async Task<ResultModel<string>> DeleteSettings(int id)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalSettings.FirstOrDefaultAsync();
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除成功");
        }
        //============================= 【2.簽核步驟】=============================
    }
}
