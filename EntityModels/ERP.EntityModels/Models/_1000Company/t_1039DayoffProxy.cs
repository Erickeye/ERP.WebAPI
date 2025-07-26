using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1039DayoffProxy
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Display(Name = "請假單編號")]
        public int DayoffId { get; set; }

        [Display(Name = "代理人Id")]
        public int ProxyId { get; set; }

        [Display(Name = "代理人是否同意")]
        public bool? ProxyAgree { get; set; }

        [Display(Name = "日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? DateTime { get; set; }

        [Display(Name = "請假人Id")]
        public int SelfId { get; set; }

        [Display(Name = "請假人是否確認")]
        public bool? IsConfirm { get; set; }
    }
}
