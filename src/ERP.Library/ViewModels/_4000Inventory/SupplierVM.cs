using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;

namespace ERP.Library.ViewModels._4000Inventory
{
    /// <summary>
    /// 查詢-供應商搜尋（分頁與排序）
    /// </summary>
    public class SupplierSearchVM : SearchModel
    {
        /// <summary>
        /// 關鍵字同時比對供應商編號、名稱與統一編號，所以不用 SearchField（它一次只能比一個欄位），由 Service 組條件
        /// </summary>
        [Display(Name = "關鍵字(供應商編號、名稱、統一編號)")]
        public string? Keyword { get; set; }

        [Display(Name = "啟用狀態(true啟用中;false已停用)")]
        [SearchField("IsActive", SearchCompare.Equal)]
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 查詢-供應商列表／單筆（不含信用餘額、最後交易日、暫付金額）
    /// </summary>
    public class SupplierVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "供應商編號")]
        public string? No { get; set; }

        [Display(Name = "名稱")]
        public string? Name { get; set; }

        [Display(Name = "統一編號")]
        public string? TaxIdNumber { get; set; }

        [Display(Name = "負責人")]
        public string? Owner { get; set; }

        [Display(Name = "聯絡電話")]
        public string? ContactPhone { get; set; }

        [Display(Name = "傳真")]
        public string? FaxPhone { get; set; }

        [Display(Name = "業務人員")]
        public string? Salesperson { get; set; }

        [Display(Name = "地址")]
        public string? Address { get; set; }

        [Display(Name = "銀行名稱")]
        public string? BankName { get; set; }

        [Display(Name = "分行名稱")]
        public string? SubBankName { get; set; }

        [Display(Name = "支票帳號")]
        public string? CheckingAccount { get; set; }

        [Display(Name = "匯款帳號")]
        public string? RemittanceAccount { get; set; }

        [Display(Name = "付款天數")]
        public int? PayDays { get; set; }

        [Display(Name = "信用額度")]
        public decimal? CreditLine { get; set; }

        [Display(Name = "是否啟用中")]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 輸入-新增／修改供應商（新增時 Id 傳 0）
    /// </summary>
    public class SupplierAddVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "供應商編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? No { get; set; }

        [Display(Name = "名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        public string? Name { get; set; }

        [Display(Name = "統一編號")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "統一編號須為 8 碼數字")]
        public string? TaxIdNumber { get; set; }

        [Display(Name = "負責人")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? Owner { get; set; }

        [Display(Name = "聯絡電話")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? ContactPhone { get; set; }

        [Display(Name = "傳真")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? FaxPhone { get; set; }

        [Display(Name = "業務人員")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? Salesperson { get; set; }

        [Display(Name = "地址")]
        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        public string? Address { get; set; }

        [Display(Name = "銀行名稱")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? BankName { get; set; }

        [Display(Name = "分行名稱")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? SubBankName { get; set; }

        [Display(Name = "支票帳號")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? CheckingAccount { get; set; }

        [Display(Name = "匯款帳號")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? RemittanceAccount { get; set; }

        [Display(Name = "付款天數")]
        [Range(0, 365, ErrorMessage = "付款天數須介於 0 到 365")]
        public int? PayDays { get; set; }

        [Display(Name = "信用額度")]
        [Range(0, 9999999999.99, ErrorMessage = "信用額度須介於 0 到 9,999,999,999.99")]
        public decimal? CreditLine { get; set; }
    }

    /// <summary>
    /// 查詢-填單用的供應商選單項目
    /// </summary>
    public class SupplierSelectVM
    {
        [Display(Name = "供應商識別碼")]
        public int Value { get; set; }

        /// <summary>
        /// 顯示用文字：供應商編號與名稱（沒有編號時只顯示名稱），讓同名的供應商也分得出來
        /// </summary>
        [Display(Name = "供應商編號與名稱")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 已停用的供應商只會在指定了 includeId 時出現，前端據此標示「已停用」
        /// </summary>
        [Display(Name = "是否啟用中")]
        public bool IsActive { get; set; }
    }
}
