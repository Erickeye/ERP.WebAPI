using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1200PettyCash
    {
        [Key]
        [Display(Name = "代墊款編號")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_ID { get; set; }

        [Display(Name = "付款對象")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Payee { get; set; }

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "必填欄位")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_PettyCash_Date { get; set; }

        [Display(Name = "請款公司")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_PettyCash_Company { get; set; }

        [Display(Name = "事由")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_PettyCash_Reason { get; set; }

        [Display(Name = "總計金額")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]
        public decimal? f_PettyCash_TotalAmount { get; set; }

        [Display(Name = "經手人")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Handler { get; set; }

        [Display(Name = "主管")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Supervisor { get; set; }

        [Display(Name = "經理")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Manager { get; set; }

        [Display(Name = "會計")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Accountant { get; set; }

        [Display(Name = "給付日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_PettyCash_PaymentDate { get; set; }

        [Display(Name = "填單人")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Filler { get; set; }

        [Display(Name = "簽核主管")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_PettyCash_Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? f_PettyCash_Approval { get; set; }

        [Display(Name = "是否會計立帳")]
        public bool? f_PettyCash_Accounting { get; set; }
    }
}
