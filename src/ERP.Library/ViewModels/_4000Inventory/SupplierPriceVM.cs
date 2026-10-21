using System.ComponentModel.DataAnnotations;

namespace ERP.Library.ViewModels._4000Inventory
{
    /// <summary>
    /// 查詢-供應商報價搜尋（某供應商之下；分頁與排序）
    /// </summary>
    public class SupplierPriceSearchVM : SearchModel
    {
        [Display(Name = "供應商流水號")]
        public int SupplierId { get; set; }

        /// <summary>
        /// 關鍵字同時比對料號與品名，由 Service 組條件
        /// </summary>
        [Display(Name = "關鍵字(料號、品名)")]
        public string? Keyword { get; set; }
    }

    /// <summary>
    /// 查詢-供應商報價列表／單筆
    /// </summary>
    public class SupplierPriceVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "供應商流水號")]
        public int SupplierId { get; set; }

        [Display(Name = "料號")]
        public string No { get; set; } = string.Empty;

        [Display(Name = "品名")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "單位")]
        public string Unit { get; set; } = string.Empty;

        [Display(Name = "單價(新台幣未稅)")]
        public decimal Price { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "最後修改時間")]
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 查詢-進貨單填單用的供應商報價（只含品名、單位、單價，供填單參考）
    /// </summary>
    public class SupplierPriceLookupVM
    {
        [Display(Name = "品名")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "單位")]
        public string Unit { get; set; } = string.Empty;

        [Display(Name = "單價(新台幣未稅)")]
        public decimal Price { get; set; }
    }

    /// <summary>
    /// 輸入-新增／修改供應商報價（新增時 Id 傳 0；修改時忽略 SupplierId，報價不能換供應商）
    /// </summary>
    public class SupplierPriceAddVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "供應商流水號")]
        public int SupplierId { get; set; }

        [Display(Name = "料號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? No { get; set; }

        [Display(Name = "品名")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        public string? Name { get; set; }

        [Display(Name = "單位")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(16, ErrorMessage = "長度不可超過 16 個字元")]
        public string? Unit { get; set; }

        [Display(Name = "單價(新台幣未稅)")]
        [Required(ErrorMessage = "必填欄位")]
        [Range(0.01, 9999999999.99, ErrorMessage = "單價須大於 0，且不可超過 9,999,999,999.99")]
        public decimal? Price { get; set; }

        [Display(Name = "備註")]
        [StringLength(256, ErrorMessage = "長度不可超過 256 個字元")]
        public string? Note { get; set; }
    }
}
