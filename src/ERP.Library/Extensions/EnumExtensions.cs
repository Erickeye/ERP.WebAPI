using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.ViewModels;

namespace ERP.Library.Extensions
{
    public static class EnumExtensions
    {
        public static List<SelectModel> ToEnumList<TEnum>(
              this TEnum _, params TEnum[] excludes)
            where TEnum : Enum
        {
            //可排除內容
            var excludeSet = excludes?.ToHashSet() ?? new HashSet<TEnum>();

            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(e => !excludeSet.Contains(e))
                .Select(e => new SelectModel
                {
                    Value = Convert.ToInt32(e),
                    Text = e.ToString() // 可改成 DisplayName
                })
                .ToList();
        }

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
        public static string GetDisplayName<T>(this object obj)
        {
            int value;

            if (obj == null)
            {
                return string.Empty;
            }

            if (!int.TryParse(obj.ToString(), out value))
            {
                value = 0;
            }

            var e = Enum.Parse(typeof(T), Convert.ToString(value));

            return ((Enum)e).GetDisplayName()!;
        }
        
    }
}
