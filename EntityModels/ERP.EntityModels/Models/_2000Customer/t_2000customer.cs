using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._2000Customer
{
    public class t_2000customer
    {
        [Display(Name = "客戶編號")]
        [Key]
        public int f_customer_ID { get; set; }

        [Display(Name = "流水號")]
        public string? f_customer_UID { get; set; }

        [Display(Name = "客戶簡稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_AttribName { get; set; }

        [Display(Name = "客戶名稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_Name { get; set; }

        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(8, ErrorMessage = "必須在8位數以內")]
        public string? f_customer_TaxInvoiceNumber { get; set; }

        [Display(Name = "負責人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_Owner { get; set; }

        [Display(Name = "連絡電話")]
        //[Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_customer_ContactPhone { get; set; }

        [Display(Name = "傳真電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_customer_FaxPhone { get; set; }

        [Display(Name = "責任業務員")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_staff_ChineseName { get; set; }

        [Display(Name = "公司地址")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "必須在50個字以內")]
        public string? f_customer_RegisteredAddress { get; set; }

        [Display(Name = "出貨地址")]
        //[Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "必須在50個字以內")]
        public string? f_customer_DeliveryAddress { get; set; }

        [Display(Name = "發票地址")]
        [StringLength(50, ErrorMessage = "必須在50個字以內")]
        public string? f_customer_TaxInvoiceAddress { get; set; }

        [Display(Name = "銀行名稱")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_BankName { get; set; }

        [Display(Name = "支票帳戶")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_CheckingAccount { get; set; }

        [Display(Name = "匯款帳戶")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_customer_RemittanceAccount { get; set; }

        [Display(Name = "付款票期")]
        [StringLength(8, ErrorMessage = "必須在8個字以內")]
        public string? f_customer_PayDays { get; set; }

        [Display(Name = "信用額度")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]

        public Nullable<decimal> f_customer_CreditLine { get; set; }

        [Display(Name = "信用餘額")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]
        public Nullable<decimal> f_customer_CreditBalance { get; set; }

        [Display(Name = "最後交易日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> f_customer_LastDeliveryDate { get; set; }

        [Display(Name = "暫收款")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]
        public Nullable<decimal> f_customer_Advance { get; set; }

        [Display(Name = "發票格式")]
        [StringLength(8, ErrorMessage = "必須在8個字以內")]
        public string? f_custome_InvoiceForm { get; set; }
    }
}
