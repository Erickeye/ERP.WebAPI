using System.Linq.Expressions;
using System.Reflection;

namespace ERP.Library.Helpers
{
    public static class SortHelper
    {
        public static Dictionary<string, Expression<Func<T, object>>> GetColumns<T>()
        {
            var dict = new Dictionary<string, Expression<Func<T, object>>>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var param = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(param, prop);

                Expression converted = prop.PropertyType.IsValueType
                    ? Expression.Convert(propertyAccess, typeof(object))
                    : propertyAccess;

                dict[prop.Name] = Expression.Lambda<Func<T, object>>(converted, param);
            }

            return dict;
        }
    }
}
