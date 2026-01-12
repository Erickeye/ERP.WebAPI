using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchFieldAttribute : Attribute
    {
        public string EntityProperty { get; }
        public SearchCompare Compare { get; }

        public SearchFieldAttribute(
            string entityProperty,
            SearchCompare compare = SearchCompare.Equal)
        {
            EntityProperty = entityProperty;
            Compare = compare;
        }
    }
    public enum SearchCompare
    {
        Equal,
        Contains,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
}
