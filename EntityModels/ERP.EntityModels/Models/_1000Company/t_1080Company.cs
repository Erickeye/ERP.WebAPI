using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1080Company
    {
        [Display(Name = "編號")]
        [Key]
        public int ID { get; set; }

        [Display(Name = "公司簡稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? AttribName { get; set; }

        [Display(Name = "公司名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Name { get; set; }

        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(8, ErrorMessage = "必須在8位數以內")]
        public string? TaxInvoiceNumber { get; set; }

        [Display(Name = "稅籍編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(9, ErrorMessage = "必須在9位數以內")]
        public string? TaxSerialNumber { get; set; }

        [Display(Name = "負責人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Owner { get; set; }

        [Display(Name = "連絡電話")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? ContactPhone { get; set; }

        [Display(Name = "傳真電話")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? FaxPhone { get; set; }

        [Display(Name = "公司地址")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(64, ErrorMessage = "必須在64個字以內")]
        public string? RegisteredAddress { get; set; }

        [Display(Name = "出貨地址")]
        //[Required(ErrorMessage = "必填欄位")]
        [StringLength(64, ErrorMessage = "必須在64個字以內")]
        public string? DeliveryAddress { get; set; }

        [Display(Name = "發票地址")]
        [StringLength(64, ErrorMessage = "必須在64個字以內")]
        public string? TaxInvoiceAddress { get; set; }

        [Display(Name = "銀行名稱")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? BankName { get; set; }

        [Display(Name = "支票帳戶")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? CheckingAccount { get; set; }

        [Display(Name = "匯款帳戶")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? RemittanceAccount { get; set; }

        [Display(Name = "付款票期")]
        [StringLength(8, ErrorMessage = "必須在8個字以內")]
        public string? PayDays { get; set; }

        [Display(Name = "公司註冊日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FoundedDate { get; set; }

        [Display(Name = "發票格式")]
        [StringLength(8, ErrorMessage = "必須在8個字以內")]
        public string? InvoiceForm { get; set; }
    }
}
