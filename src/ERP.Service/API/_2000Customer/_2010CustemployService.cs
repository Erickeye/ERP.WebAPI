using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._2000Customer;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._2000Customer
{
    public interface I_2010CustemployService
    {
        Task<ResultModel<ListResult<CustemployListVM>>> Index(int customerId);
        Task<ResultModel<CustemployInputVM>> Get(int id);
        Task<ResultModel<string>> CreateOrEdit(CustemployInputVM data);
        Task<ResultModel<string>> Delete(int id);
    }
    public class _2010CustemployService : I_2010CustemployService
    {
        private readonly ERPContext _context;

        public _2010CustemployService(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<CustemployListVM>>> Index(int customerId)
        {
            var result = new ResultModel<ListResult<CustemployListVM>>();
            var list = await _context.t_2010Custemploy
                .Where(x => x.CustomerId == customerId)
                .Select(x => new CustemployListVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Department = x.Department,
                    JobTitle = x.JobTitle,
                    Job = x.Job,
                    ExtNum = x.ExtNum,
                    MobilePhone = x.MobilePhone,
                    Account = x.Account,
                    Email = x.Email,
                    MarriageStatus = (MarriageStatus)x.MarriageStatus!,
                    JobStatus = (JobStatus)x.JobStatus!,
                    Momo = x.Momo
                })
                .ToListAsync();
            return result;
        }
        public async Task<ResultModel<CustemployInputVM>> Get(int id)
        {
            var entity = await _context.t_2010Custemploy
                .Select(x => new CustemployInputVM
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    Name = x.Name,
                    Department = x.Department,
                    JobTitle = x.JobTitle,
                    Job = x.Job,
                    ExtNum = x.ExtNum,
                    MobilePhone = x.MobilePhone,
                    Account = x.Account,
                    Email = x.Email,
                    MarriageStatus = (MarriageStatus)x.MarriageStatus!,
                    JobStatus = (JobStatus)x.JobStatus!,
                    Momo = x.Momo
                })
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            return ResultModel.Ok(entity);
        }
        public async Task<ResultModel<string>> CreateOrEdit(CustemployInputVM data)
        {            
            var entity = await _context.t_2010Custemploy.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (entity == null)
            {
                entity = new t_2010Custemploy();
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            ObjectHelper.CopyProperties(data, entity, "Custemploy");
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料成功修改");
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = _context.t_2010Custemploy.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除");
        }
    }
}
