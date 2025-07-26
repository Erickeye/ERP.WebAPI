using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1020PerformanceTarget
    {
        [Display(Name = "流水號")]
        [Key]

        public int f_PTarget_ID { get; set; }
        [Display(Name = "員工編號")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_UID { get; set; }
        [Display(Name = "員工中文名")]
        [Required(ErrorMessage = "必填欄位")]
        [Compare(nameof(t_1000Staff.f_staff_ChineseName))]
        public string? f_staff_ChineseName { get; set; }
        [Display(Name = "年度")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime f_PTarget_Annyal { get; set; }
        [Display(Name = "業績目標")]
        [Required(ErrorMessage = "必填欄位")]
        public decimal? f_PTarget_Achieve { get; set; }
    }
}
