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
using Org.BouncyCastle.Asn1.X509;

namespace ERP.Service.API._1000Company
{
    public interface I_1030DayoffService
    {
        Task<ResultModel<string>> Create(DayOffInputVM data);
        Task<ResultModel<string>> Edit(DayOffInputVM data);
        Task<ResultModel<ListResult<DayOffListVM>>> Index(DayOffIndexSearch search);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<double>> GetTheYearSpecialLeaveDays(int staffId);
        Task<ResultModel<double>> GetTheYearSpecialTotalDays(int staffId);
        Task<ResultModel<double>> GetRemainSpecialDays(int staffId);
    }
    public class _1030DayoffService : I_1030DayoffService
    {
        private readonly ERPContext _context;

        public _1030DayoffService(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<string>> Create(DayOffInputVM vm)
        {
            // 新增
            var entity = new t_1030Dayoff
            {
                ApplicationDate = DateTime.Now
            };
            _context.Add(entity);

            var result = await Modify(vm, entity);
            if (!result.IsSuccess)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }

            return ResultModel.Ok("資料新增成功");
        }
        public async Task<ResultModel<string>> Edit(DayOffInputVM vm)
        {
            //有資料 => 修改
            var entity = _context.t_1030Dayoff.FirstOrDefault(c => c.Id == vm.Id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }

            var result = await Modify(vm, entity);
            if (!result.IsSuccess)
            {
                return ResultModel.Error(result.ErrorCode, result.ErrorMessage);
            }
            return ResultModel.Ok("資料修改成功");
        }
        private async Task<ResultModel<string>> Modify(DayOffInputVM vm, t_1030Dayoff entity)
        {
            //尚未離職的員工
            var staffList = await _context.t_1000Staff
                .Where(x => x.ResignationDate == null || x.ResignationDate > DateTime.Now)
                .Select(x => x.StaffId)
                .ToArrayAsync();

            var vmStaffs = new int[] { (int)vm.Proxy!, (int)vm.Applicant!, (int)vm.LeaveTaker! };

            if (!vmStaffs.All(x => staffList.Contains(x)))
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData, "『請假人』、『申請者』、『代理人』其中找不到該【員工】。");
            }

            entity.ApplicationDate = vm.ApplicationDate;
            entity.LeaveTaker = (int)vm.LeaveTaker!;
            entity.Applicant = vm.Applicant;
            entity.Proxy = vm.Proxy;
            entity.LeaveType = (int)vm.LeaveType!;
            entity.Reason = vm.Reason;
            entity.ProxySignature = vm.ProxySignature;
            entity.BeginDate = (DateTime)vm.BeginDate!;
            entity.EndDate = (DateTime)vm.EndDate!;

            await _context.SaveChangesAsync();

            return ResultModel.Ok();
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
        public async Task<ResultModel<double>> GetRemainSpecialDays(int staffId)
        {
            //取得今年總計可以休的特休假
            var totalDaysResult = await GetTheYearSpecialTotalDays(staffId);
            var totalDays = totalDaysResult.Data;
            //取得今年已請特休假總計
            var specialLeaveDaysResult = await GetTheYearSpecialLeaveDays(staffId);
            if (!specialLeaveDaysResult)
            {
                return ResultModel.Error(specialLeaveDaysResult.ErrorCode, specialLeaveDaysResult.ErrorMessage);
            }
            var specialLeaveDays = totalDaysResult.Data;

            return ResultModel.Ok(totalDays - specialLeaveDays);
        }
    }
}
