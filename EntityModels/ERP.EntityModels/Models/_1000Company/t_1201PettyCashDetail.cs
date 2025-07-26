using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int f_PettyCashDetail_ID { get; set; }

        [Display(Name = "代墊款編號")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_PettyCash_ID { get; set; }

        [Display(Name = "項目")]
        public string? f_PettyCashDetail_Project { get; set; }

        [Display(Name = "名稱")]
        public string? f_PettyCashDetail_Name { get; set; }

        [Display(Name = "會計科目")]
        public string? f_PettyCashDetail_PurchaseID { get; set; }

        [Display(Name = "使用者/車號")]
        public string? f_PettyCashDetail_Vehicle { get; set; }

        [Display(Name = "供應商")]
        public string? f_PettyCashDetail_Supplier { get; set; }

        [Display(Name = "統一編號")]
        public string? f_PettyCashDetail_Content { get; set; }


        [Display(Name = "發票號碼")]
        public string? f_PettyCashDetail_InvoiceNumber { get; set; }

        [Display(Name = "發票日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_PettyCashDetail_InvoiceDate { get; set; }

        [Display(Name = "金額")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 99999999, ErrorMessage = "必須在12位數以內")]
        public decimal? f_PettyCashDetail_Amount { get; set; }

        [Display(Name = "稅金")]
        public decimal? f_PettyCashDetail_Tax { get; set; }

        [Display(Name = "合計")]
        public decimal? f_PettyCashDetail_Total { get; set; }

        [Display(Name = "排序")]
        public int? f_PettyCashDetail_Sort { get; set; }
    }
}
