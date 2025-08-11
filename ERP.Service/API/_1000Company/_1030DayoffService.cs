using ERP.Data;
using ERP.EntityModels.Models._1000Company;
using ERP.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API._1000Company
{
    public class _1030DayoffService
    {
        private readonly ERPContext _context;

        public _1030DayoffService(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<string>> CreateOrEdit(t_1030Dayoff data)
        {
            var result = new ResultModel<string>();
            //var entity = _context.t_1030Dayoff.Where(c => c.Id)
            return result;
        }
    }
}
