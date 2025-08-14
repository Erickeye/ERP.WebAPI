using ERP.Data;
using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API._1000Company
{
    public interface I_1030DayoffService
    {
        Task<ResultModel<string>> CreateOrEdit(DayOffInputVM data);
        Task<ResultModel<ListResult<DayOffListVM>>> Index();
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<double>> GetRemainSpecialDays(int staffId);
    }
    public class _1030DayoffService : I_1030DayoffService
    {
        private readonly ERPContext _context;

        public _1030DayoffService(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<string>> CreateOrEdit(DayOffInputVM data)
        {
            var result = new ResultModel<string>();
            var entity = _context.t_1030Dayoff.FirstOrDefault(c => c.Id == data.Id);
            if (entity == null)
            {
                //找不到資料 => 新增
                entity = new t_1030Dayoff();
                MapToEntity(data, entity);
                _context.Add(entity);
                result.SetSuccess("資料成功新增");
            }
            else
            {
                //有資料 => 修改
                MapToEntity(data, entity);
                result.SetSuccess("資料成功修改");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<ListResult<DayOffListVM>>> Index()
        {
            var result = new ResultModel<ListResult<DayOffListVM>>();
            var list = await _context.t_1030Dayoff
                .Select(x => new DayOffListVM
                {
                    LeaveTaker = x.Staff_LeaveTaker!.ChineseName!,
                    ApplicationDate = x.ApplicationDate,
                    Proxy = x.Staff_Proxy!.ChineseName,
                    LeaveType = x.LeaveType,
                    Reason = x.Reason,
                    BeginDate = x.BeginDate,
                    EndDate = x.EndDate
                })
                .ToListAsync();
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var entity = await _context.t_1030Dayoff.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料刪除成功");
            return result;
        }
        public async Task<ResultModel<double>> GetRemainSpecialDays(int staffId)
        {
            var result = new ResultModel<double>();
            //今年已請特休數量
            var currentYear = DateTime.Now.Year;
            var startOfYear = new DateTime(currentYear, 1, 1);
            var endOfYear = new DateTime(currentYear, 12, 31);

            var list = await _context.t_1030Dayoff!
           .Where(c => c.LeaveType == LeaveType.特休 &&
                       c.LeaveTaker == staffId &&
                       c.BeginDate >= startOfYear &&
                       c.BeginDate <= endOfYear)
           .ToListAsync();

            var dayOffHours = list
           .Sum(c =>
           {
               var begin = c.BeginDate;
               var end = c.EndDate;

               // 請假總時數
               var total = (end - begin).TotalHours;

               // 午休時間：12:30 - 13:30
               var restStart = new DateTime(begin.Year, begin.Month, begin.Day, 12, 30, 0);
               var restEnd = new DateTime(begin.Year, begin.Month, begin.Day, 13, 30, 0);

               // 計算重疊時間（取交集）
               var overlapStart = (begin > restStart) ? begin : restStart;
               var overlapEnd = (end < restEnd) ? end : restEnd;

               if (overlapEnd > overlapStart)
               {
                   var overlap = (overlapEnd - overlapStart).TotalHours;
                   total -= overlap; // 扣除實際重疊時間（最多 1 小時）
               }

               return total;
           });
            var dayOff = dayOffHours / 8.0; // 每 8 小時算 1 天
            result.SetSuccess(dayOff);
            return result;
        }

        public void MapToEntity(DayOffInputVM source, t_1030Dayoff target)
        {
            target.ApplicationDate = source.ApplicationDate;
            target.LeaveTaker = (int)source.LeaveTaker!;
            target.Applicant = source.Applicant;
            target.Proxy = source.Proxy;
            target.LeaveType = source.LeaveType;
            target.Reason = source.Reason;
            target.ProxySignature = source.ProxySignature;
            target.BeginDate = (DateTime)source.BeginDate!;
            target.EndDate = (DateTime)source.EndDate!;
        }
    }
}
