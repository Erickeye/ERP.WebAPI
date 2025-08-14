using ERP.Data;
using ERP.EntityModels.Models.Other;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.Approval;
using ERP.Models.AMS;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
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
        Task<ResultModel<ListResult<ApprovalSettings>>> CheckSettings();
        Task<ResultModel<string>> CreateOrEditSettings(ApprovalSettings data);
        Task<ResultModel<string>> DeleteSettings(int id);
        Task<ResultModel<ListResult<ApprovalStep>>> CheckStep(int approvalSettingsId);
        Task<ResultModel<string>> CreateOrEditStep(ApprovalStep data);
        Task<ResultModel<string>> DeleteStep(int id);
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
            var result = new ResultModel<string>();

            int roleId = _currentUserService.RoleId;
            int userId = _currentUserService.UserId;
            //填表人預設通過
            var record = new ApprovalRecord
            {
                //ApprovalStepId = 0,
                TableId = data.TableId,
                StepOrder = 0,
                Status = ApprovalStatus.Approved,
                RoleId = roleId,
                UserId = userId,
                Date = DateTime.Now
            };
            _context.Add(record);

            var steps = _context.ApprovalStep
               .Where(x => x.ApprovalSettingsId == data.ApprovalSettingsId)
               .ToList();

            //該簽核設定底下的每個流程
            foreach (var step in steps)
            {
                switch (step.Mode)
                {
                    case ApprovalMode.Specify:
                        //指定人員
                        var approvalUser = _context.ApprovalStepNumber
                        .Where(x => x.ApprovalStepId == step.Id)
                        .ToList();
                        foreach (var item in approvalUser)
                        {
                            _context.Add(new ApprovalRecord
                            {
                                ApprovalStepId = step.Id,
                                TableId = data.TableId,
                                StepOrder = step.StepOrder,
                                Status = ApprovalStatus.Pending,
                                UserId = item.UserId
                            });
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
                        });
                        break;
                    case ApprovalMode.Customized:
                        //自訂人數
                        _context.Add(new ApprovalRecord
                        {
                            ApprovalStepId = step.Id,
                            TableId = data.TableId,
                            StepOrder = step.StepOrder,
                            Status = ApprovalStatus.Pending,
                        });
                        break;
                }
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<ListResult<ApprovalSettings>>> CheckSettings()
        {
            var result = new ResultModel<ListResult<ApprovalSettings>>();
            var list = await _context.ApprovalSettings.ToListAsync();
            result.Data = new ListResult<ApprovalSettings>(list);
            return result;
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
                result.SetSuccess("資料成功新增");
            }
            else
            {
                _context.Entry(entity).CurrentValues.SetValues(data);
                result.SetSuccess("資料成功修改");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> DeleteSettings(int id)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalSettings.FirstOrDefaultAsync();
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料已刪除成功");
            return result;
        }
        public async Task<ResultModel<ListResult<ApprovalStep>>> CheckStep(int approvalSettingsId)
        {
            var result = new ResultModel<ListResult<ApprovalStep>>();
            var list = await _context.ApprovalStep
                .Where(x => x.ApprovalSettingsId == approvalSettingsId)
                .ToListAsync();
            if (list.Count == 0)
            {
                result.SetError(ErrorCodeType.NotFoundData, "該權限尚未設定簽核步驟");
                return result;
            }
            result.Data = new ListResult<ApprovalStep>(list);
            return result;
        }
        public async Task<ResultModel<string>> CreateOrEditStep(ApprovalStep data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalStep
                .FirstOrDefaultAsync(x => x.Id == data.Id);
            if (entity == null)
            {
                entity = new ApprovalStep();
                _context.Add(data);
                result.SetSuccess("資料成功新增");
            }
            else
            {
                _context.Entry(entity).CurrentValues.SetValues(data);
                result.SetSuccess("資料成功修改");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> DeleteStep(int id)
        {
            var result = new ResultModel<string>();
            var entity = await _context.ApprovalStep.FirstOrDefaultAsync();
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料已刪除成功");
            return result;
        }

    }
}
