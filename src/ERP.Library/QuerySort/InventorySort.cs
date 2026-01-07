using System.Linq.Expressions;
using ERP.Library.ViewModels._4000Inventory;

namespace ERP.Library.QuerySort
{
    public static class InventorySort
    {
        public static readonly Dictionary<string, Expression<Func<InventoryVM, object>>> Columns
            = new()
            {
            { "Category",   x => x.Category! },
            { "Number",     x => x.Number! },
            { "Name",       x => x.Name! },
            { "SupplierName", x => x.SupplierName! },
            { "LocationName", x => x.LocationName! },
            };
    }

}
