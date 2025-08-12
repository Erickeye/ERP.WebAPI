using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Helpers
{
    public static class ObjectHelper
    {
        public static void CopyProperties<TSource, TTarget>(TSource source, TTarget target, params string[] ignoreProperties)
        {
            if (source == null || target == null)
                return;

            var ignoreList = new HashSet<string>(ignoreProperties, StringComparer.OrdinalIgnoreCase);

            var sourceProps = typeof(TSource).GetProperties().Where(p => p.CanRead);
            var targetProps = typeof(TTarget).GetProperties().Where(p => p.CanWrite);

            foreach (var sourceProp in sourceProps)
            {
                //排除掉不需要的屬性
                if (ignoreList.Contains(sourceProp.Name))
                    continue;

                var targetProp = targetProps.FirstOrDefault(tp => tp.Name == sourceProp.Name
                                                                && tp.PropertyType.IsAssignableFrom(sourceProp.PropertyType));
                if (targetProp != null)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }
        }
    }
}
