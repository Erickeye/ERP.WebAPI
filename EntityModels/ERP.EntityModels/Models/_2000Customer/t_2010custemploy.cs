using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._2000Customer
{
    public class t_2010custemploy
    {
        [Key]
        [Display(Name = "客戶排序")]

        public int f_custemploy_ID { get; set; }

        [Display(Name = "客戶編號")]
        [Required(ErrorMessage = "必填欄位")]
        public int? f_customer_ID { get; set; }

        [Display(Name = "公司簡稱")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_customer_AttribName { get; set; }

        [Display(Name = "公司名稱")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_customer_Name { get; set; }

        [Display(Name = "聯絡人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_Name { get; set; }

        [Display(Name = "部門")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_Department { get; set; }

        [Display(Name = "職稱")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_Post { get; set; }

        [Display(Name = "主要工作")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_Job { get; set; }

        [Display(Name = "分機號碼")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_ExtNum { get; set; }

        [Display(Name = "行動電話")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_MobilePhone { get; set; }

        [Display(Name = "使用者帳號")]
        [StringLength(20, ErrorMessage = "必須在20個字以內")]
        public string? f_custemploy_Account { get; set; }

        [Display(Name = "電子郵件")]
        [StringLength(50, ErrorMessage = "必須在50個字以內")]
        public string? f_custemploy_Email { get; set; }

        [Display(Name = "婚姻狀況")]
        [StringLength(10, ErrorMessage = "必須在10個字以內")]
        public string? f_custemploy_EmotionState { get; set; }

        [Display(Name = "在職狀況")]
        [StringLength(100, ErrorMessage = "必須在100個字以內")]
        public string? f_custemploy_Remark { get; set; }

        [Display(Name = "備註")]
        [StringLength(100, ErrorMessage = "必須在100個字以內")]
        public string? f_custemploy_Momo { get; set; }
    }
}
