using ERP.Data;
using ERP.EntityModels.Models._9000Other;
using ERP.EntityModels.Models.Other;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.Approval;
using ERP.Library.ViewModels.UserInfo;
using ERP.Models.AMS;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API
{
    public interface IApprovalService
    {
        Task<ResultModel<string>> SendApprovalProcess(SendApprovalProcessVM data);
        Task<ResultModel<string>> Approval(ApprovalVM data);
        Task<ResultModel<string>> RejectApproval(ApprovalVM data);
        Task<ResultModel<ListResult<ApprovalSettings>>> CheckSettings();
        Task<ResultModel<string>> CreateOrEditSettings(ApprovalSettings data);
        Task<ResultModel<string>> DeleteSettings(int id);
        Task<ResultModel<ListResult<ApprovalStep>>> CheckStep(int approvalSettingsId);
        Task<ResultModel<string>> CreateOrEditStep(ApprovakStepInputVM data);
        Task<ResultModel<string>> DeleteStep(int id);
        Task<ResultModel<ListResult<ApprovalStepNumber>>> CheckStepNumber(int ApprovalStepId);
        Task<ResultModel<string>> CreateOrEditStepNumber(ApprovalStepNumberInputVM data);
        Task<ResultModel<string>> DeleteStepNumber(int id);
    }
    public class ApprovalService : IApprovalService
    {
        private readonly ERPContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ApprovalService(ERPContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<ResultModel<string>> SendApprovalProcess(SendApprovalProcessVM data)
        {
            //檢查設定檔
            var settingList = await _context.ApprovalSettings
                .Where(x => x.TableType == data.TableType &&
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
            var existTableId = await _context.ApprovalRecord
                .AnyAsync(x => x.TableId == data.TableId &&
                               x.TableType == setting.TableType &&
                               (x.Status == ApprovalStatus.Pending));
            if (existTableId)
            {
                return ResultModel.Error(ErrorCodeType.ApprovalExists);
            }

            //var existTableId = await _context.ApprovalRecord.AnyAsync(x => x.TableId == data.TableId);
            //if (existTableId) {
            //    //如果單號重複 => 檢查TableType是否一樣
            //    var checkSettingType = await _context.ApprovalRecord
            //    .Include(x => x.ApprovalStep)                  // record → step
            //    .ThenInclude(step => step.ApprovalSettings)    // step → settings
            //    .AnyAsync(x => x.ApprovalStep.ApprovalSettings.Id == setting.Id &&
            //        x.ApprovalStep.ApprovalSettings.TableType == setting.TableType);

            //    if (checkSettingType)
            //    {
            //        result.SetError(ErrorCodeType.ApprovalExists);
            //        return result;
            //    }
            //}

            var tableCheckers = new Dictionary<TableType, Func<string, Task<bool>>>
            {
                { TableType.請假單, async id => await _context.t_1030Dayoff.AnyAsync(x => x.Id.ToString() == id) },
                { TableType.公文,   async id => await _context.t_1040Document.AnyAsync(x => x.Id.ToString() == id) }
            };
            //檢查是否有該單號
            if (tableCheckers.TryGetValue(setting.TableType, out var checker))
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
                Status = ApprovalStatus.Approved,
                RoleId = roleId,
                UserId = userId,
                Date = DateTime.Now,
                Memo = "申請者自動通過",
                TableType = data.TableType
            };
            _context.Add(fitstRecord);

            var steps = _context.ApprovalStep
               .Where(x => x.ApprovalSettingsId == setting.Id)
               .ToList();

            int stepCount = 0;
            //該簽核設定底下的每個流程
            foreach (var step in steps)
            {
                stepCount++;
                switch (step.Mode)
                {
                    case ApprovalMode.Specify:
                        //指定人員
                        var approvalUser = _context.ApprovalStepNumber
                        .Where(x => x.ApprovalStepId == step.Id)
                        .ToList();
                        foreach (var item in approvalUser)
                        {
                            var recordx = new ApprovalRecord
                            {
                                ApprovalStepId = step.Id,
                                RoleId = step.RoleId,
                                TableId = data.TableId,
                                StepOrder = step.StepOrder,
                                Status = ApprovalStatus.Pending,
                                UserId = item.UserId,
                                TableType = data.TableType
                            };
                            //訊息通知
                            var notification = new Notification
                            {
                                DateTime = DateTime.Now,
                                Type = setting.TableType,
                                TargetId = data.TableId,
                                UserId = item.UserId
                            };
                            //step整個簽核訊息先建立，但只顯示step1的訊息通知
                            if (stepCount == 1)
                            {
                                notification.IsShow = true;
                                fitstRecord.ApprovalStepId = step.Id;
                            }
                            _context.Add(recordx);
                            _context.Add(notification);
                        }
                        break;
                    case ApprovalMode.Single:
                        //單人
                        _context.Add(new ApprovalRecord
                        {
                            ApprovalStepId = step.Id,
                            TableId = data.TableId,
                            StepOrder = step.StepOrder,
                            Status = ApprovalStatus.Pending,
                            TableType = data.TableType
                        });
                        break;
                    case ApprovalMode.Customized:
                        //自訂人數
                        for (int i = 0; i < step.RequiredCounts; i++)
                        {
                            _context.Add(new ApprovalRecord
                            {
                                ApprovalStepId = step.Id,
                                TableId = data.TableId,
                                StepOrder = step.StepOrder,
                                Status = ApprovalStatus.Pending,
                                TableType = data.TableType
                            });
                        }
                        break;
                }
            }
            await _context.SaveChangesAsync();
            return ResultModel.Ok("簽核流程成功送出");
        }
        public async Task<ResultModel<string>> Approval(ApprovalVM data)
        {
            int userId = _currentUserService.UserId;

            var record = await _context.ApprovalRecord
                .FirstOrDefaultAsync(x => x.TableType == data.TableType &&
                                          x.TableId == data.TableId &&
                                          x.UserId == userId &&
                                          x.StepOrder >= 1 &&
                                          x.Status == ApprovalStatus.Pending);
            if (record == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該簽核內容");
            }
            // 檢查有無前面尚未完成的簽核
            var pendingPrevious = await _context.ApprovalRecord
                .AnyAsync(x => x.TableType == data.TableType &&
                               x.TableId == data.TableId &&
                               x.StepOrder >= 1 &&
                               x.StepOrder < record.StepOrder &&
                               x.Status == ApprovalStatus.Pending);
            if (pendingPrevious)
            {
                return ResultModel.Error(ErrorCodeType.NotYetTurnForApprovalStep);
            }

            //身分檢查
            //if (record.UserId != userId) {
            //    result.SetError(ErrorCodeType.InvalidUserOperation);
            //    return result;
            //}
            //簽核狀態檢查
            if (record.Status == ApprovalStatus.Approved)
            {                
                return ResultModel.Error(ErrorCodeType.IsAlreadyApproval);
            }

            record.Date = DateTime.Now;
            record.Status = ApprovalStatus.Approved;
            record.Memo = data.Memo;

            //該簽核還有其他使用者未簽核
            var isPendingCount = await _context.ApprovalRecord
                .Where(x => x.ApprovalStepId == record.ApprovalStepId &&
                    x.StepOrder == record.StepOrder &&
                    x.Status == ApprovalStatus.Pending).CountAsync();
            //只剩一筆代表目前階段只剩下當前這筆Record
            if (isPendingCount > 1)
            {
                await _context.SaveChangesAsync();
                return ResultModel.Ok("簽核成功，該階段等待其他人員簽核");
            }
            //簽核階段全部完成(沒有下一個階段的Step階段)
            var nextStep = _context.ApprovalRecord
                .Where(x => x.TableId == record.TableId &&
                    x.StepOrder == record.StepOrder + 1);
            if (nextStep.Count() == 0)
            {
                await _context.SaveChangesAsync();
                return ResultModel.Ok("簽核成功，簽核作業已全數完成");
            }
            //該階段完成但還有下個階段
            //找出下個階段的通知訊息
            var nextUsersNotification = _context.Notification
                .Where(x => x.TargetId == record.TableId && x.IsShow == false);
            var nextUsers = nextStep.Select(x => x.UserId).ToList();

            var notificationDict = nextUsersNotification.ToDictionary(x => x.UserId);

            foreach (var user in nextUsers)
            {
                if (!notificationDict.TryGetValue((int)user!, out var notification))
                {
                    return ResultModel.Error(ErrorCodeType.UserNotFound);
                }
                //開啟下個階段的通知訊息
                notification.IsShow = true;
            }
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
                .FirstOrDefaultAsync(x => x.TableType == data.TableType &&
                                          x.TableId == data.TableId &&
                                          x.UserId == userId &&
                                          x.StepOrder >= 1 &&
                                          x.Status == ApprovalStatus.Pending);
            if (record == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到該簽核內容");
            }
            // 檢查有無前面尚未完成的簽核
            var pendingPrevious = await _context.ApprovalRecord
                .AnyAsync(x => x.TableType == data.TableType &&
                               x.TableId == data.TableId &&
                               x.StepOrder >= 1 &&
                               x.StepOrder < record.StepOrder &&
                               x.Status == ApprovalStatus.Pending);
            if (pendingPrevious)
            {
                return ResultModel.Error(ErrorCodeType.NotYetTurnForApprovalStep);
            }

            var recordList = await _context.ApprovalRecord
                .Where(x => x.TableType == data.TableType &&
                            x.TableId == data.TableId &&
                            x.Status == ApprovalStatus.Pending)
                .ToListAsync();
            foreach (var recordItem in recordList)
            {
                if (recordItem.Status == ApprovalStatus.Pending)
                {
                    recordItem.Status = ApprovalStatus.GetRejected;
                }
            }

            record.Date = DateTime.Now;
            record.Status = ApprovalStatus.Rejected;
            record.Memo = data.Memo;

            await _context.SaveChangesAsync();
            return ResultModel.Ok("已拒絕該簽核作業");
        }
        public async Task<ResultModel<ListResult<ApprovalSettings>>> CheckSettings()
        {
            var result = new ResultModel<ListResult<ApprovalSettings>>();
            var list = await _context.ApprovalSettings.ToListAsync();
            return ResultModel.Ok(list);
        }
        public async Task<ResultModel<string>> CreateOrEditSettings(ApprovalSettings data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalSettings
                .FirstOrDefaultAsync(x => x.Id == data.Id);
            if (entity == null)
            {
                entity = new ApprovalSettings();
                _context.Add(data);
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            else
            {
                _context.Entry(entity).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功修改");
            }
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
        public async Task<ResultModel<ListResult<ApprovalStep>>> CheckStep(int approvalSettingsId)
        {
            var result = new ResultModel<ListResult<ApprovalStep>>();
            var list = await _context.ApprovalStep
                .Where(x => x.ApprovalSettingsId == approvalSettingsId)
                .ToListAsync();
            if (list.Count == 0)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "該權限尚未設定簽核步驟");
            }
            result.Data = new ListResult<ApprovalStep>(list);
            return result;
        }
        public async Task<ResultModel<string>> CreateOrEditStep(ApprovakStepInputVM data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalStep
                .FirstOrDefaultAsync(x => x.Id == data.Id);
            if (entity == null)
            {
                var lastStep = _context.ApprovalStep
                    .Where(x => x.ApprovalSettingsId == data.ApprovalSettingsId)
                    .OrderByDescending(x => x.StepOrder)
                    .FirstOrDefault();
                int stepOrder = lastStep == null
                    ? 1
                    : lastStep.StepOrder + 1;
                _context.Add(new ApprovalStep
                {
                    ApprovalSettingsId = data.ApprovalSettingsId,
                    StepOrder = stepOrder,
                    RoleId = data.RoleId,
                    Mode = data.Mode,
                    RequiredCounts = data.RequiredCounts
                });
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            else
            {
                entity.RoleId = data.RoleId;
                entity.Mode = data.Mode;
                entity.RequiredCounts = data.RequiredCounts;
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功修改");
            }
        }
        public async Task<ResultModel<string>> DeleteStep(int id)
        {
            var entity = await _context.ApprovalStep.FirstOrDefaultAsync();
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除成功");
        }
        public async Task<ResultModel<ListResult<ApprovalStepNumber>>> CheckStepNumber(int ApprovalStepId)
        {
            var result = new ResultModel<ListResult<ApprovalStepNumber>>();
            var list = await _context.ApprovalStepNumber
                .Where(x => x.ApprovalStepId == ApprovalStepId)
                .ToListAsync();
            if (list.Count == 0)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "該權限尚未設定簽核步驟成員");
            }
            result.Data = new ListResult<ApprovalStepNumber>(list);
            return result;
        }
        public async Task<ResultModel<string>> CreateOrEditStepNumber(ApprovalStepNumberInputVM data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalStepNumber
                .FirstOrDefaultAsync(x => x.Id == data.Id);
            if (entity == null)
            {
                _context.Add(new ApprovalStepNumber
                {
                    ApprovalStepId = data.ApprovalStepId,
                    UserId = data.UserId
                });
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            else
            {
                entity.UserId = data.UserId;
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功修改");
            }
        }
        public async Task<ResultModel<string>> DeleteStepNumber(int id)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalStepNumber.FirstOrDefaultAsync();
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除成功");
        }
    }
}
