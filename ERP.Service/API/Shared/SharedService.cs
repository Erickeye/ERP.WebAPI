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
    public interface ISharedService
    {
        ResultModel<List<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum;
    }
    public class SharedService : ISharedService
    {
        public ResultModel<List<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum
        {
            var result = new ResultModel<List<SelectModel>>();
            var list = EnumHelper.ToSelectList<TEnum>();
            result.SetSuccess(list);
            return result;
        }
    }
}
