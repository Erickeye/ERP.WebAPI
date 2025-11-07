using ERP.Data;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.UserInfo;
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
        ResultModel<ListResult<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum;
    }
    public class SharedService : ISharedService
    {
        public ResultModel<ListResult<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum
        {
            var result = new ResultModel<List<SelectModel>>();
            var list = EnumHelper.ToSelectList<TEnum>();
            return ResultModel.Ok(list);
        }
    }
}
