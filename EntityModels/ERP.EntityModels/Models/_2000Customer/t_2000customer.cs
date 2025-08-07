using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._2000Customer
{
    public class t_2000Customer
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [StringLength(32)]
        [Display(Name = "客戶簡稱")]
        public string? AttribName { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "客戶名稱")]
        public string Name { get; set; } = null!;

        [Required, StringLength(8)]
        [Display(Name = "統一編號")]
        public string? TaxInvoiceNumber { get; set; }

        [Required, StringLength(32)]
        [Display(Name = "負責人")]
        public string? Owner { get; set; }

        [StringLength(32)]
        [Display(Name = "連絡電話")]
        public string? ContactPhone { get; set; }

        [StringLength(32)]
        [Display(Name = "傳真電話")]
        public string? FaxPhone { get; set; }

        [StringLength(32)]
        [Display(Name = "責任業務員")]
        public string? StaffChineseName { get; set; }

        [Required, StringLength(64)]
        [Display(Name = "公司地址")]
        public string? RegisteredAddress { get; set; }

        [StringLength(64)]
        [Display(Name = "出貨地址")]
        public string? DeliveryAddress { get; set; }

        [StringLength(64)]
        [Display(Name = "發票地址")]
        public string? TaxInvoiceAddress { get; set; }

        [StringLength(32)]
        [Display(Name = "銀行名稱")]
        public string? BankName { get; set; }

        [StringLength(32)]
        [Display(Name = "支票帳戶")]
        public string? CheckingAccount { get; set; }

        [StringLength(32)]
        [Display(Name = "匯款帳戶")]
        public string? RemittanceAccount { get; set; }

        [StringLength(8)]
        [Display(Name = "付款票期")]
        public string? PayDays { get; set; }

        [Range(0, 999999999999)]
        [Display(Name = "信用額度")]
        public decimal? CreditLine { get; set; }

        [Range(0, 999999999999)]
        [Display(Name = "信用餘額")]
        public decimal? CreditBalance { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "最後交易日")]
        public DateTime? LastDeliveryDate { get; set; }

        [Range(0, 999999999999)]
        [Display(Name = "暫收款")]
        public decimal? Advance { get; set; }

        [Display(Name = "發票格式")]
        public InvoiceFormType? InvoiceForm{ get; set; }

        public virtual ICollection<t_2010Custemploy> StaffCertificates { get; set; } = new List<t_2010Custemploy>();
    }
}
