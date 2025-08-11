using ERP.Data;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels;
using ERP.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API.Shared
{
    public class SharedService
    {
        private readonly ERPContext _context;

        public SharedService(ERPContext context)
        {
            _context = context;
        }
        public async Task<ResultModel<List<SelectModel>>> GetMarriageStatus()
        {
            var result = new ResultModel<List<SelectModel>>();
            var items = EnumHelper.ToSelectList<MarriageStatus>();
            result.SetSuccess(items);
            return result;
        }
    }
}
