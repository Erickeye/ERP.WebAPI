using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels._4000Inventory
{
    public class InventorySearchVM
    {
        [SwaggerParameter("供應商")]
        public string? SupplierName { get; set; }

        [SwaggerParameter("位置")]
        public string? LocationName { get; set; }

        [SwaggerParameter("類別")]
        public string? Category { get; set; }

        [SwaggerParameter("名稱")]
        public string? Name { get; set; }
    }
    public class InventoryVM
    {
        public int Id { get; set; }

        [SwaggerParameter("供應商Id")]
        public int SupplierId { get; set; }

        [SwaggerParameter("供應商名稱")]
        public string? SupplierName { get; set; }

        [SwaggerParameter("商品名稱")]
        public string? Name { get; set; } = null!;

        [SwaggerParameter("位置Id")]
        public int LocationId { get; set; }

        [SwaggerParameter("位置名稱")]
        public string? LocationName { get; set; }

        [SwaggerParameter("類別")]
        public string? Category { get; set; }

        [SwaggerParameter("最後買進日")]
        public DateTime? LastPurchaseDate { get; set; }

        [SwaggerParameter("編號")]
        public string? Number { get; set; }

        [SwaggerParameter("單位")]
        public string? Unit { get; set; }

        [SwaggerParameter("數量")]
        public decimal? Quantity { get; set; }

        [SwaggerParameter("價格")]
        public decimal? Amount { get; set; }

        [SwaggerParameter("總計")]
        public decimal? Total { get; set; }
    }
}
