using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1100Department
    {
        [Key]
        [Display(Name = "部門代碼")]
        [StringLength(16, ErrorMessage = "必須在16位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? Id { get; set; }

        [Display(Name = "部門名稱")]
        [StringLength(32, ErrorMessage = "必須在32位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? Name { get; set; }

        public virtual ICollection<t_1101DepartmentUnit> DepartmentUnit { get; set; } = new List<t_1101DepartmentUnit>();
    }
}
