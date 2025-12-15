using ERP.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.Helpers
{
    public static class EnumHelper
    {
        public static ResultModel<ListResult<SelectModel>> GetEnumList<TEnum>(params TEnum[] excludes) where TEnum : Enum
        {
            var list = ToSelectList<TEnum>(excludes);
            return ResultModel.Ok(list);
        }

        public static List<SelectModel> ToSelectList<TEnum>(params TEnum[] excludes) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectModel
                       {
                           Value = Convert.ToInt32(e),
                           Text = e.ToString() // 或用 DisplayName
                       })
                       .ToList();
        }
    }
}
