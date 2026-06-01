using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ERP.Approval.Services;

public class ApprovalWorkflowService : IApprovalWorkflowService
{
    public const string CompletedMessage = "簽核完成，全部流程已結束";

    private readonly ERPDbContext _context;
    private readonly IApprovalUserContext _userContext;
    private readonly IApprovalTargetHandlerResolver _targetHandlerResolver;

    public ApprovalWorkflowService(
        ERPDbContext context,
        IApprovalUserContext userContext,
        IApprovalTargetHandlerResolver targetHandlerResolver)
    {
        _context = context;
        _userContext = userContext;
        _targetHandlerResolver = targetHandlerResolver;
    }

    public async Task<ResultModel<string>> SendApprovalProcess(ApprovalVM data)
    {
        var settingList = await _context.ApprovalSettings
            .Include(x => x.ApprovalStep)
                .ThenInclude(x => x.ApprovalStepNumber)
            .Where(x => x.TableType == (int)data.TableType && x.IsActive)
            .ToListAsync();

        if (settingList.Count == 0)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到啟用中的簽核設定");
        }

        if (settingList.Count > 1)
        {
            return ResultModel.Error(ErrorCodeType.MultipleApprovalSettingsExists);
        }

        var setting = settingList.First();

        var lastRecord = await _context.ApprovalRecord
            .Where(x => x.TableId == data.TableId && x.TableType == setting.TableType)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        if (lastRecord != null &&
            (lastRecord.Status == (int)ApprovalStatus.Pending ||
             lastRecord.Status == (int)ApprovalStatus.Approved))
        {
            return ResultModel.Error(ErrorCodeType.ApprovalExists);
        }

        var targetHandler = _targetHandlerResolver.Resolve(data.TableType);
        if (targetHandler == null)
        {
            return ResultModel.Error(ErrorCodeType.InvalidApproval, "不支援的簽核資料類型");
        }

        if (!await targetHandler.ExistsAsync(data.TableId))
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData);
        }

        var firstRecord = new ApprovalRecord
        {
            ApprovalStepId = 0,
            TableId = data.TableId,
            StepOrder = 0,
            Status = (int)ApprovalStatus.Approved,
            RoleId = _userContext.RoleId,
            UserId = _userContext.UserId,
            Date = DateTime.Now,
            Memo = "送出簽核",
            TableType = (int)data.TableType
        };
        _context.Add(firstRecord);

        var steps = setting.ApprovalStep
            .OrderBy(x => x.StepOrder)
            .ToList();

        for (var index = 0; index < steps.Count; index++)
        {
            var step = steps[index];
            if (index == 0)
            {
                firstRecord.ApprovalStepId = step.Id;
            }

            switch (step.Mode)
            {
                case (int)ApprovalMode.Specify:
                    foreach (var item in step.ApprovalStepNumber)
                    {
                        _context.Add(new ApprovalRecord
                        {
                            ApprovalStepId = step.Id,
                            TableId = data.TableId,
                            StepOrder = step.StepOrder,
                            Status = (int)ApprovalStatus.Pending,
                            UserId = item.UserId,
                            TableType = (int)data.TableType
                        });
                    }
                    break;

                case (int)ApprovalMode.Role:
                    for (var i = 0; i < step.RequiredCounts; i++)
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
        return ResultModel.Ok($"單據 {data.TableId} 已送出簽核");
    }

    public async Task<ResultModel<ListResult<GetApprovalProgressVM>>> GetApprovalProgress(ApprovalVM data)
    {
        var records = await _context.ApprovalRecord
            .Where(x => x.TableId == data.TableId && x.TableType == (int)data.TableType)
            .ToListAsync();

        if (!records.Any())
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到簽核紀錄");
        }

        var userDict = await _context.User.ToDictionaryAsync(x => x.Id, x => x.Name);
        var roleDict = await _context.Role.ToDictionaryAsync(x => x.Id, x => x.RoleName);

        var vm = records
            .OrderBy(x => x.StepOrder)
            .ThenBy(x => x.Id)
            .Select(x => new GetApprovalProgressVM
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
                RoleName = x.RoleId.HasValue && roleDict.TryGetValue(x.RoleId.Value, out var roleName)
                    ? roleName
                    : null,
                Status = x.Status,
                StatusDisplay = x.Status.GetDisplayName<ApprovalStatus>(),
                ApprovalTime = x.Date,
                Memo = x.Memo
            })
            .ToList();

        return ResultModel.Ok(vm);
    }

    public async Task<ResultModel<string>> Approval(ApprovalVM data)
    {
        var records = await _context.ApprovalRecord
            .Where(x => x.TableType == (int)data.TableType &&
                        x.TableId == data.TableId &&
                        x.StepOrder >= 1 &&
                        x.Status == (int)ApprovalStatus.Pending)
            .ToListAsync();

        var record = records.FirstOrDefault(x =>
            x.UserId == _userContext.UserId || x.RoleId == _userContext.RoleId);

        if (record == null)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到可簽核的紀錄");
        }

        var pendingPrevious = records.Any(x => x.StepOrder < record.StepOrder);
        if (pendingPrevious)
        {
            return ResultModel.Error(ErrorCodeType.NotYetTurnForApprovalStep);
        }

        record.Date = DateTime.Now;
        record.Status = (int)ApprovalStatus.Approved;
        record.Memo = data.Memo;

        var sameStepPendingCount = records.Count(x => x.StepOrder == record.StepOrder);
        if (sameStepPendingCount > 1)
        {
            await _context.SaveChangesAsync();
            return ResultModel.Ok("簽核成功，等待同層其他簽核人員");
        }

        var nextStep = records.Where(x => x.StepOrder == record.StepOrder + 1);
        if (!nextStep.Any())
        {
            await _context.SaveChangesAsync();
            return ResultModel.Ok(CompletedMessage);
        }

        await _context.SaveChangesAsync();
        return ResultModel.Ok("簽核成功，已進入下一關");
    }

    public async Task<ResultModel<string>> RejectApproval(ApprovalVM data)
    {
        if (string.IsNullOrWhiteSpace(data.Memo))
        {
            return ResultModel.Error(ErrorCodeType.IncompleteInfo, "請填寫駁回原因");
        }

        var record = await _context.ApprovalRecord
            .Where(x => x.TableType == (int)data.TableType &&
                        x.TableId == data.TableId &&
                        x.UserId == _userContext.UserId &&
                        x.StepOrder >= 1 &&
                        x.Status == (int)ApprovalStatus.Pending)
            .FirstOrDefaultAsync();

        if (record == null)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到可駁回的簽核紀錄");
        }

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
            recordItem.Status = (int)ApprovalStatus.GetRejected;
        }

        record.Date = DateTime.Now;
        record.Status = (int)ApprovalStatus.Rejected;
        record.Memo = data.Memo;

        await _context.SaveChangesAsync();
        return ResultModel.Ok("已駁回簽核");
    }

    public async Task<ResultModel<string>> RevokeApproval(ApprovalVM data)
    {
        var recordList = await _context.ApprovalRecord
            .Where(x => x.TableType == (int)data.TableType && x.TableId == data.TableId)
            .OrderByDescending(x => x.StepOrder)
            .ToListAsync();

        var record = recordList.FirstOrDefault();
        if (record == null)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到簽核紀錄");
        }

        if (record.Status != (int)ApprovalStatus.Pending)
        {
            return ResultModel.Error(ErrorCodeType.InvalidUserOperation, "目前狀態不可撤回簽核");
        }

        if (record.UserId != _userContext.UserId)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "只能撤回自己的簽核");
        }

        foreach (var recordItem in recordList.Where(x => x.Status == (int)ApprovalStatus.Pending))
        {
            recordItem.Status = (int)ApprovalStatus.GetRevoked;
        }

        await _context.SaveChangesAsync();
        return ResultModel.Ok("已撤回簽核");
    }

    public async Task<ResultModel<string>> VoidApproval(ApprovalVM data)
    {
        var recordList = await _context.ApprovalRecord
            .Where(x => x.TableType == (int)data.TableType &&
                        x.TableId == data.TableId &&
                        x.Status != (int)ApprovalStatus.GetRevoked)
            .OrderByDescending(x => x.StepOrder)
            .ToListAsync();

        var record = recordList.FirstOrDefault();
        if (!recordList.Any() || record == null)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "找不到簽核紀錄");
        }

        if (record.UserId != _userContext.UserId)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData, "只能作廢自己的簽核");
        }

        foreach (var recordItem in recordList)
        {
            if (recordItem.Status != (int)ApprovalStatus.Approved)
            {
                return ResultModel.Error(ErrorCodeType.InvalidUserOperation, "簽核尚未全部通過，不可作廢");
            }

            recordItem.Status = (int)ApprovalStatus.GetRevoked;
        }

        await _context.SaveChangesAsync();
        return ResultModel.Ok("已作廢簽核");
    }

    public async Task<ResultModel<ListResult<GetApprovalNotifyVM>>> GetApprovalNotify()
    {
        var query = _context.ApprovalRecord.AsQueryable();

        var records = await query
            .Where(x =>
                (x.UserId == _userContext.UserId || x.RoleId == _userContext.RoleId) &&
                x.Status == (int)ApprovalStatus.Pending &&
                (
                    x.StepOrder == 1 ||
                    query.Any(p =>
                        p.TableId == x.TableId &&
                        p.TableType == x.TableType &&
                        p.StepOrder == x.StepOrder - 1 &&
                        p.Status == (int)ApprovalStatus.Approved)
                ))
            .Select(x => new GetApprovalNotifyVM
            {
                Id = x.Id,
                TableType = x.TableType,
                TableName = x.TableType.GetDisplayName<TableType>(),
                TableId = x.TableId
            })
            .ToListAsync();

        return ResultModel.Ok(records);
    }
}
