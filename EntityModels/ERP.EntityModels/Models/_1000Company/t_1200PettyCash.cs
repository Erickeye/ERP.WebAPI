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
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? PettyCashId { get; set; }

        [Display(Name = "付款對象")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? Payee { get; set; }

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "必填欄位")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "請款公司")]
        [StringLength(64, ErrorMessage = "必須在64位數以內")]
        public string? Company { get; set; }

        [Display(Name = "事由")]
        [StringLength(64, ErrorMessage = "必須在64位數以內")]
        public string? Reason { get; set; }

        [Display(Name = "總計金額")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "主管")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? Supervisor { get; set; }

        [Display(Name = "給付日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PaymentDate { get; set; }

        [Display(Name = "填單人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? Filler { get; set; }

        [Display(Name = "簽核主管")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        public string? Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? Approval { get; set; }

        [Display(Name = "是否會計立帳")]
        public bool? Accounting { get; set; }

        public ICollection<t_1201PettyCashDetail> PettyCashDetails { get; set; } = new List<t_1201PettyCashDetail>();
    }
}
