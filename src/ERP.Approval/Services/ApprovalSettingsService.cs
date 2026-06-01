using ERP.Approval.Abstractions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ERP.Approval.Services;

public class ApprovalSettingsService : IApprovalSettingsService
{
    private readonly ERPDbContext _context;

    public ApprovalSettingsService(ERPDbContext context)
    {
        _context = context;
    }

    public async Task<ResultModel<ListResult<ApprovalSettings>>> SettingsIndex()
    {
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
                Steps = x.ApprovalStep
                    .OrderBy(s => s.StepOrder)
                    .Select(s => new ApprovalStepVM
                    {
                        Id = s.Id,
                        StepOrder = s.StepOrder,
                        RoleId = s.RoleId,
                        Mode = s.Mode,
                        ModeDisplay = s.Mode.GetDisplayName<ApprovalMode>(),
                        RequiredCounts = s.RequiredCounts,
                        StepNumbers = s.ApprovalStepNumber
                            .Select(n => new ApprovalStepNumberVM
                            {
                                Id = n.Id,
                                ApprovalStepId = n.ApprovalStepId,
                                UserId = n.UserId
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

        return ResultModel.Ok("資料新增成功");
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

        if (vm.Steps == null ||
            vm.Steps.Any(x => x.StepOrder == 0) ||
            vm.Steps.GroupBy(x => x.StepOrder).Any(g => g.Count() > 1))
        {
            return ResultModel.Error(ErrorCodeType.FieldValueIsInvalid, "簽核關卡設定有誤");
        }

        entitySetting.TableType = vm.TableType;
        entitySetting.Name = vm.Name ?? entitySetting.Name;
        entitySetting.IsActive = vm.IsActive;

        var deleteSteps = entitySetting.ApprovalStep
            .Where(x => !vm.Steps.Any(c => c.Id == x.Id))
            .ToList();

        foreach (var step in deleteSteps)
        {
            _context.RemoveRange(step.ApprovalStepNumber);
            _context.Remove(step);
        }

        foreach (var stepVm in vm.Steps)
        {
            var stepEntity = entitySetting.ApprovalStep.FirstOrDefault(x => x.Id == stepVm.Id);
            if (stepEntity == null)
            {
                stepEntity = new ApprovalStep();
                entitySetting.ApprovalStep.Add(stepEntity);
            }

            stepEntity.RoleId = stepVm.RoleId;
            stepEntity.StepOrder = stepVm.StepOrder;
            stepEntity.Mode = stepVm.Mode;
            stepEntity.RequiredCounts = stepVm.RequiredCounts;

            var deleteNumbers = stepEntity.ApprovalStepNumber
                .Where(x => !stepVm.StepNumbers.Any(n => n.Id == x.Id))
                .ToList();

            foreach (var num in deleteNumbers)
            {
                _context.Remove(num);
            }

            foreach (var numVm in stepVm.StepNumbers)
            {
                var numEntity = stepEntity.ApprovalStepNumber.FirstOrDefault(x => x.Id == numVm.Id);
                if (numEntity == null)
                {
                    numEntity = new ApprovalStepNumber();
                    stepEntity.ApprovalStepNumber.Add(numEntity);
                }

                numEntity.UserId = numVm.UserId;
            }
        }

        await _context.SaveChangesAsync();
        return ResultModel.Ok();
    }

    public async Task<ResultModel<string>> DeleteSettings(int id)
    {
        var entity = await _context.ApprovalSettings.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return ResultModel.Error(ErrorCodeType.NotFoundData);
        }

        _context.Remove(entity);
        await _context.SaveChangesAsync();

        return ResultModel.Ok("資料刪除成功");
    }
}
