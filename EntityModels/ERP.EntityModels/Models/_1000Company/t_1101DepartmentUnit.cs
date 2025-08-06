using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1101DepartmentUnit
    {
        [Key]
        [Display(Name = "請假單編號")]
        public int Id { get; set; }

        [ForeignKey(nameof(Staff))]
        [Display(Name = "員工編號")]
        [Required(ErrorMessage = "必填欄位")]
        public int StaffId { get; set; }

        [ForeignKey(nameof(Department))]
        [Display(Name = "部門代碼")]
        [StringLength(16, ErrorMessage = "必須在16位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string DepartmentId { get; set; } = null!;

        [Display(Name = "是否為部門主管")]
        [Required(ErrorMessage = "必填欄位")]
        public bool IsManager { get; set; }

        [Display(Name = "職稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(16, ErrorMessage = "必須在16個字以內")]
        public string JobTitle { get; set; } = null!;

        public virtual t_1000Staff Staff { get; set; } = null!;
        public virtual t_1100Department Department { get; set; } = null!;
    }
}
