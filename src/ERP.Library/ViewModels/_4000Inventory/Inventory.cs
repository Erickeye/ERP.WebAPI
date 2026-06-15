using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels._4000Inventory
{
    public class InventorySearchVM : SearchModel, IDownloadFileModel
    {
        [Display(Name = "供應商名稱")]
        [SearchField("Supplier.Name", SearchCompare.Contains)]
        public string? SupplierName { get; set; }

        [Display(Name = "庫位")]
        [SearchField("Location.Name", SearchCompare.Contains)]
        public string? LocationName { get; set; }

        [Display(Name = "類別")]
        [SearchField("Category", SearchCompare.Equal)]
        public string? Category { get; set; }

        [Display(Name = "品名")]
        [SearchField("Name", SearchCompare.Contains)]
        public string? Name { get; set; }

        /// <summary>
        /// 下載格式
        /// </summary>
        [Display(Name = "下載格式(Pdf:13;Xlsx:6;預設為Xlsx)")]
        public int SaveFormat { get; set; } = 6;
    }

    public class InventoryVM
    {
        public int Id { get; set; }

        [Display(Name = "供應商Id")]
        public int SupplierId { get; set; }

        [Display(Name = "供應商名稱")]
        public string? SupplierName { get; set; }

        [Display(Name = "品名")]
        public string? Name { get; set; } = null!;

        [Display(Name = "庫位Id")]
        public int LocationId { get; set; }

        [Display(Name = "庫位")]
        public string? LocationName { get; set; }

        [Display(Name = "類別")]
        public string? Category { get; set; }

        [Display(Name = "最後進貨日")]
        public DateTime? LastPurchaseDate { get; set; }

        [Display(Name = "編號")]
        public string? No { get; set; }

        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "數量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "金額")]
        public decimal? Amount { get; set; }

        [Display(Name = "總計")]
        public decimal? Total { get; set; }
    }

    public class InventoryExportVM
    {
        public List<InventoryExportItemVM> Items { get; set; } = new();
    }

    public class InventoryExportItemVM
    {
        [Display(Name = "供應商名稱")]
        public string? SupplierName { get; set; }

        [Display(Name = "品名")]
        public string? Name { get; set; } = null!;

        [Display(Name = "庫位")]
        public string? LocationName { get; set; }

        [Display(Name = "類別")]
        public string? Category { get; set; }

        [Display(Name = "最後進貨日")]
        public string? LastPurchaseDate { get; set; }

        [Display(Name = "編號")]
        public string? No { get; set; }

        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "數量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "金額")]
        public decimal? Amount { get; set; }

        [Display(Name = "總計")]
        public decimal? Total { get; set; }
    }
}
