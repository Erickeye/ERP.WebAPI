using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1201PettyCashDetail
    {
        [Key]
        [Display(Name = "流水號")]
        [Required(ErrorMessage = "必填欄位")]
        public int PettyCashDetailId { get; set; }

        [ForeignKey("PettyCash")]
        [Display(Name = "代墊款編號")]
        [Required(ErrorMessage = "必填欄位")]
        public string PettyCashId { get; set; } = null!;

        [Display(Name = "項目")]
        public string? PettyCashDetailProject { get; set; }

        [Display(Name = "名稱")]
        [Required(ErrorMessage = "必填欄位")]
        public string? PettyCashDetailName { get; set; }

        [Display(Name = "會計科目")]
        public string? PettyCashDetailPurchaseId { get; set; }

        [Display(Name = "使用者/車號")]
        public string? PettyCashDetailVehicle { get; set; }

        [Display(Name = "供應商")]
        public string? PettyCashDetailSupplier { get; set; }

        [Display(Name = "統一編號")]
        public string? PettyCashDetailContent { get; set; }

        [Display(Name = "發票號碼")]
        public string? PettyCashDetailInvoiceNumber { get; set; }

        [Display(Name = "發票日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PettyCashDetailInvoiceDate { get; set; }

        [Display(Name = "金額")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 99999999, ErrorMessage = "必須在12位數以內")]
        public decimal? PettyCashDetailAmount { get; set; }

        [Display(Name = "稅金")]
        public decimal? PettyCashDetailTax { get; set; }

        [Display(Name = "合計")]
        public decimal? PettyCashDetailTotal { get; set; }

        [Display(Name = "排序")]
        public int? PettyCashDetailSort { get; set; }

        public t_1200PettyCash? PettyCash { get; set; }
    }
}
