using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1100Department
    {
        [Key]
        [Display(Name = "部門代碼")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_deprtmt_ID { get; set; }
        [Display(Name = "部門名稱")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_deprtmt_Name { get; set; }
    }
}
