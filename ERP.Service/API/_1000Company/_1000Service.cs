using ERP.Data;
using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ERP.Service.API._1000Company
{
    public interface I_1000Service
    {
        Task<ResultModel<List<StaffIndex>>> GetStaffIndex(string deptID, bool isResignation);
    }
    public class _1000Service : I_1000Service
    {
        private readonly ERPContext _context;

        public _1000Service(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<List<StaffIndex>>> GetStaffIndex(string? deptID, bool isResignation) { 
            var result = new ResultModel<List<StaffIndex>>();

            IQueryable<StaffIndex> query;

            //未離職員工
            if (!isResignation)
            {
                if (!string.IsNullOrEmpty(deptID))
                {
                    // Inner Join
                    query = _context.t_1000Staff!
                        .Join(
                            _context.t_1101Deprtmt!.Where(d => d.f_deprtmt_ID == deptID),
                            staff => staff.f_staff_UID,
                            dept => dept.f_staff_UID,
                            (staff, dept) => staff
                        )
                        .Select(StaffSelector());
                }
                else
                {
                    // Left Join + where dept == null
                    query = _context.t_1000Staff!
                        .GroupJoin(
                            _context.t_1101Deprtmt!,
                            staff => staff.f_staff_UID,
                            dept => dept.f_staff_UID,
                            (staff, deptGroup) => new { staff, deptGroup }
                        )
                        .SelectMany(
                            x => x.deptGroup.DefaultIfEmpty(),
                            (x, dept) => new { x.staff, dept }
                        )
                        .Where(x => x.dept == null)
                         .Select(x => x.staff)
                         .Select(StaffSelector());
                }
            }
            else
            {
                // 取得已離職員工
                query = _context.t_1000Staff!
                    .Where(q => q.f_staff_ResignationDay.HasValue)
                    .Select(StaffSelector());
            }
            if (query == null)
            {
                result.SetError(0, "找不到資料");
                return result;
            }
            result.Data = await query.ToListAsync();

            return result;
        }

        private static Expression<Func<t_1000Staff, StaffIndex>> StaffSelector()
        {
            return staff => new StaffIndex
            {
                StaffUid = staff.f_staff_UID,
                Name = staff.f_staff_ChineseName,
                IdCard = staff.f_staff_IDCard,
                Gender = staff.f_staff_Gender,
                Bitrthday = staff.f_staff_Bitrthday,
                ContactPhone = staff.f_staff_ContactPhone,
                LineId = staff.f_staff_LineID,
                Email = staff.f_staff_Email,
                ContactAddress = staff.f_staff_ContactAddress
            };
        }
    }
}
