using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var displayAttr = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false)
                                              .FirstOrDefault() as DisplayAttribute;
                if (displayAttr != null && !string.IsNullOrWhiteSpace(displayAttr.Name))
                {
                    return displayAttr.Name;
                }
            }
            return enumValue.ToString();
        }
    }
}
