using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels._4000Inventory
{
    public class InventorySearchVM : SearchModel, IDownloadFileModel
    {
        [SwaggerParameter("供應商")]
        [SearchField("Supplier.Name", SearchCompare.Contains)]
        public string? SupplierName { get; set; }

        [SwaggerParameter("位置")]
        [SearchField("Location.Name", SearchCompare.Contains)]
        public string? LocationName { get; set; }

        [SwaggerParameter("類別")]
        [SearchField("Category", SearchCompare.Equal)]
        public string? Category { get; set; }

        [SwaggerParameter("名稱")]
        [SearchField("Name", SearchCompare.Contains)]
        public string? Name { get; set; }

        /// <summary>
        /// 下載類型(Aspose.Cells.SaveFormat)
        /// </summary>
        [SwaggerSchema("下載類型(Pdf:13;Xlsx:6;預設為Xlsx)")]
        public int SaveFormat { get; set; } = 6;
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
        public string? No { get; set; }

        [SwaggerParameter("單位")]
        public string? Unit { get; set; }

        [SwaggerParameter("數量")]
        public decimal? Quantity { get; set; }

        [SwaggerParameter("價格")]
        public decimal? Amount { get; set; }

        [SwaggerParameter("總計")]
        public decimal? Total { get; set; }
    }
    public class InventoryExportVM
    {
        public List<InventoryExportItemVM> Items { get; set; } = new();
    }
    public class InventoryExportItemVM
    {

        [SwaggerParameter("供應商名稱")]
        public string? SupplierName { get; set; }

        [SwaggerParameter("商品名稱")]
        public string? Name { get; set; } = null!;

        [SwaggerParameter("位置名稱")]
        public string? LocationName { get; set; }

        [SwaggerParameter("類別")]
        public string? Category { get; set; }

        [SwaggerParameter("最後買進日")]
        public string? LastPurchaseDate { get; set; }

        [SwaggerParameter("編號")]
        public string? No { get; set; }

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
