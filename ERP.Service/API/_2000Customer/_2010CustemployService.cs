using ERP.Data;
using ERP.EntityModels.Models._2000Customer;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._2000Customer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    MarriageStatus = x.MarriageStatus,
                    JobStatus = x.JobStatus,
                    Momo = x.Momo
                })
                .ToListAsync();
            return result;
        }
        public async Task<ResultModel<CustemployInputVM>> Get(int id)
        {
            var result = new ResultModel<CustemployInputVM>();
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
                    MarriageStatus= x.MarriageStatus,
                    JobStatus = x.JobStatus,
                    Momo = x.Momo
                })
                .FirstOrDefaultAsync();
            if(entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            result.Data = entity;
            return result;
        }
        public async Task<ResultModel<string>> CreateOrEdit(CustemployInputVM data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.t_2010Custemploy.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (entity == null)
            {
                entity = new t_2010Custemploy();
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                _context.Add(entity);
                result.SetSuccess("資料成功新增");
            }
            else
            {
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                result.SetSuccess("資料修改新增");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var entity = _context.t_2010Custemploy.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料已刪除");
            return result;
        }
    }
}
