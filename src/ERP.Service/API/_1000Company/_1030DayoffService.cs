using System.Linq;
using System.Linq.Expressions;
using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.Extensions;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._1000Company
{
    public interface I_1030DayoffService
    {
        Task<ResultModel<string>> CreateOrEdit(DayOffInputVM data);
        Task<ResultModel<ListResult<DayOffListVM>>> Index(DayOffIndexSearch search);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<double>> GetTheYearSpecialLeaveDays(int staffId);
        Task<ResultModel<double>> GetTheYearSpecialTotalDays(int staffId);
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
            //找不到資料 => 新增
            var entity = new t_1030Dayoff();
            MapToEntity(data, entity);
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料新增成功");
        }
        public async Task<ResultModel<string>> Edit(DayOffInputVM data)
        {
            var entity = _context.t_1030Dayoff.FirstOrDefault(c => c.Id == data.Id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            //有資料 => 修改
            MapToEntity(data, entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料修改成功");
        }
        public async Task<ResultModel<ListResult<DayOffListVM>>> Index(DayOffIndexSearch search)
        {
            Expression<Func<t_1030Dayoff, bool>> filter = x => true;

            if (search.StartDate.HasValue)
            {
                var date = search.StartDate.Value;
                filter = filter.ExpressionAnd(x => x.BeginDate >= date);
            }
            if (search.EndDate.HasValue)
            {
                var date = search.EndDate.Value.To235959();
                filter = filter.ExpressionAnd(x => x.EndDate >= date);
            }

            var list = await _context.t_1030Dayoff
                .AsNoTracking()
                .Where(filter)
                .Select(x => new DayOffListVM
                {
                    LeaveTaker = x.LeaveTakerNavigation!.ChineseName!,
                    ApplicationDate = x.ApplicationDate,
                    Proxy = x.ProxyNavigation!.ChineseName,
                    LeaveType = (LeaveType)x.LeaveType,
                    Reason = x.Reason,
                    BeginDate = x.BeginDate,
                    EndDate = x.EndDate
                })
                .ToListAsync();

            return ResultModel.Ok(list);
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = await _context.t_1030Dayoff.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料刪除成功");
        }
        public async Task<ResultModel<double>> GetTheYearSpecialLeaveDays(int staffId)
        {
            //今年已請特休數量
            var currentYear = DateTime.Now.Year;
            var startOfYear = new DateTime(currentYear, 1, 1);
            var endOfYear = new DateTime(currentYear, 12, 31);

            var list = await _context.t_1030Dayoff!
           .Where(c => (LeaveType)c.LeaveType == LeaveType.特休 &&
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
            return ResultModel.Ok(dayOff);
        }
        public async Task<ResultModel<double>> GetTheYearSpecialTotalDays(int staffId)
        {
            var staff = await _context.t_1000Staff
                .Where(x => x.StaffId == staffId)
                .FirstOrDefaultAsync();

            if (staff == null)
            {
                return ResultModel.Error(ErrorCodeType.DataEmpty, "找不到該員工。");
            }

            //取得員工入職日
            var onBoardDate = staff.OnBoardDate;
            var today = DateTime.Today;

            // 計算滿幾年
            var years = today.Year - onBoardDate.Year;
            if (onBoardDate.AddYears(years) > today)
            {
                years--;
            }

            // 計算是否滿 6 個月
            var isOverSixMonths = onBoardDate.AddMonths(6) <= today;

            double specialDays = 0;

            if (years < 1)
            {
                specialDays = isOverSixMonths ? 3 : 0;
            }
            else if (years < 2)
            {
                specialDays = 7;
            }
            else if (years < 3)
            {
                specialDays = 10;
            }
            else if (years < 5)
            {
                specialDays = 14;
            }
            else if (years < 10)
            {
                specialDays = 15;
            }
            else
            {
                // 10 年以上：每年 +1，上限 30 天
                specialDays = Math.Min(15 + (years - 10), 30);
            }

            return ResultModel.Ok(specialDays);
        }

        public void MapToEntity(DayOffInputVM source, t_1030Dayoff target)
        {
            target.ApplicationDate = source.ApplicationDate;
            target.LeaveTaker = (int)source.LeaveTaker!;
            target.Applicant = source.Applicant;
            target.Proxy = source.Proxy;
            target.LeaveType = (int)source.LeaveType;
            target.Reason = source.Reason;
            target.ProxySignature = source.ProxySignature;
            target.BeginDate = (DateTime)source.BeginDate!;
            target.EndDate = (DateTime)source.EndDate!;
        }
    }
}
