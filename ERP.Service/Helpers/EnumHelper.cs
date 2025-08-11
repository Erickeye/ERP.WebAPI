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
        public static List<SelectModel> ToSelectList<TEnum>() where TEnum : Enum
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
