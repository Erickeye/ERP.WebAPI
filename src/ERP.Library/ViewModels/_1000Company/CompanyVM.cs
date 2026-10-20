using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;

namespace ERP.Library.ViewModels._1000Company
{
    /// <summary>
    /// 查詢-公司搜尋（分頁與排序）
    /// </summary>
    public class CompanySearchVM : SearchModel
    {
        /// <summary>
        /// 關鍵字同時比對公司簡稱、公司全名與統一編號，所以不用 SearchField（它一次只能比一個欄位），由 Service 組條件
        /// </summary>
        [Display(Name = "關鍵字(公司簡稱、公司全名、統一編號)")]
        public string? Keyword { get; set; }

        [Display(Name = "啟用狀態(true啟用中;false已停用)")]
        [SearchField("IsActive", SearchCompare.Equal)]
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// 查詢-公司列表／單筆
    /// </summary>
    public class CompanyVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "公司簡稱")]
        public string? AttribName { get; set; }

        [Display(Name = "公司全名")]
        public string? Name { get; set; }

        [Display(Name = "統一編號")]
        public string? TaxInvoiceNumber { get; set; }

        [Display(Name = "稅籍編號")]
        public string? TaxSerialNumber { get; set; }

        [Display(Name = "負責人")]
        public string? Owner { get; set; }

        [Display(Name = "聯絡電話")]
        public string? ContactPhone { get; set; }

        [Display(Name = "傳真")]
        public string? FaxPhone { get; set; }

        [Display(Name = "登記地址")]
        public string? RegisteredAddress { get; set; }

        [Display(Name = "送貨地址")]
        public string? DeliveryAddress { get; set; }

        [Display(Name = "發票地址")]
        public string? TaxInvoiceAddress { get; set; }

        [Display(Name = "銀行名稱")]
        public string? BankName { get; set; }

        [Display(Name = "支票帳號")]
        public string? CheckingAccount { get; set; }

        [Display(Name = "匯款帳號")]
        public string? RemittanceAccount { get; set; }

        [Display(Name = "付款天數")]
        public string? PayDays { get; set; }

        [Display(Name = "成立日期")]
        public DateTime? FoundedDate { get; set; }

        [Display(Name = "發票格式")]
        public string? InvoiceForm { get; set; }

        [Display(Name = "是否啟用中")]
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 輸入-新增／修改公司（新增時 Id 傳 0）
    /// </summary>
    public class CompanyAddVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "公司簡稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? AttribName { get; set; }

        [Display(Name = "公司全名")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? Name { get; set; }

        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "必填欄位")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "統一編號須為 8 碼數字")]
        public string? TaxInvoiceNumber { get; set; }

        [Display(Name = "稅籍編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "稅籍編號須為 9 碼")]
        public string? TaxSerialNumber { get; set; }

        [Display(Name = "負責人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? Owner { get; set; }

        [Display(Name = "聯絡電話")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? ContactPhone { get; set; }

        [Display(Name = "傳真")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? FaxPhone { get; set; }

        [Display(Name = "登記地址")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? RegisteredAddress { get; set; }

        [Display(Name = "送貨地址")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? DeliveryAddress { get; set; }

        [Display(Name = "發票地址")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? TaxInvoiceAddress { get; set; }

        [Display(Name = "銀行名稱")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? BankName { get; set; }

        [Display(Name = "支票帳號")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? CheckingAccount { get; set; }

        [Display(Name = "匯款帳號")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string? RemittanceAccount { get; set; }

        [Display(Name = "付款天數")]
        [StringLength(8, ErrorMessage = "長度不可超過 8 個字元")]
        public string? PayDays { get; set; }

        [Display(Name = "成立日期")]
        public DateTime? FoundedDate { get; set; }

        [Display(Name = "發票格式")]
        [StringLength(8, ErrorMessage = "長度不可超過 8 個字元")]
        public string? InvoiceForm { get; set; }
    }

    /// <summary>
    /// 查詢-填單用的公司選單項目
    /// </summary>
    public class CompanySelectVM
    {
        [Display(Name = "公司識別碼")]
        public int Value { get; set; }

        /// <summary>
        /// 顯示用文字：公司簡稱
        /// </summary>
        [Display(Name = "公司簡稱")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 已停用的公司只會在指定了 includeId 時出現，前端據此標示「已停用」
        /// </summary>
        [Display(Name = "是否啟用中")]
        public bool IsActive { get; set; }
    }
}
