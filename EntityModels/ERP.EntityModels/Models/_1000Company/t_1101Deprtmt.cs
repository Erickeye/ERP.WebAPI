using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1101Deprtmt
    {
        [Required(ErrorMessage = "必填欄位")]
        [Compare(nameof(t_1100Department.f_deprtmt_ID))]
        [Display(Name = "部門代碼")]
        public string? f_deprtmt_ID { get; set; }
        [Display(Name = "部門名稱")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        [Compare(nameof(t_1100Department.f_deprtmt_Name))]
        public string? f_deprtmt_Name { get; set; }

        [Key]
        [Display(Name = "員工編號")]
        [Required(ErrorMessage = "必填欄位")]
        [Compare(nameof(t_1000Staff.f_staff_UID))]
        public string? f_staff_UID { get; set; }
        [Display(Name = "員工中文名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        [Compare(nameof(t_1000Staff.f_staff_ChineseName))]
        public string? f_staff_ChineseName { get; set; }
        [Display(Name = "部門主管")]
        [Required(ErrorMessage = "必填欄位")]
        public bool? f_deprtmt_MG { get; set; }

        [Display(Name = "職稱")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_deprtmt_Seniority { get; set; }

    }
}
