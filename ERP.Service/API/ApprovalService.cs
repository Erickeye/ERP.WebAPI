using ERP.Data;
using ERP.EntityModels.Models.Other;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Models.AMS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API
{
    public interface IApprovalService
    {
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

        public ApprovalService(ERPContext context)
        {
            _context = context;
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
            if (entity == null) {
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
            if (entity == null) {
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
            if (list.Count == 0) {
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
