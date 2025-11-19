using ERP.Library.Enums._1000Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._2000Customer
{
    public class CustemployListVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public string? Job { get; set; }
        public string? ExtNum { get; set; }
        public string? MobilePhone { get; set; }
        public string? Account { get; set; }
        public string? Email { get; set; }
        public MarriageStatus? MarriageStatus { get; set; }
        public JobStatus? JobStatus { get; set; } = 0;
        public string? Momo { get; set; }
    }
    public class CustemployInputVM
    {
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Display(Name = "客戶編號")]
        [Required(ErrorMessage = "必填欄位")]
        public int CustomerId { get; set; }

        [Display(Name = "聯絡人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Name { get; set; }

        [Display(Name = "部門")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Department { get; set; }

        [Display(Name = "職稱")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? JobTitle { get; set; }

        [Display(Name = "主要工作")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Job { get; set; }

        [Display(Name = "分機號碼")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? ExtNum { get; set; }

        [Display(Name = "行動電話")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? MobilePhone { get; set; }

        [Display(Name = "使用者帳號")]
        [StringLength(32, ErrorMessage = "必須在32個字以內")]
        public string? Account { get; set; }

        [Display(Name = "電子郵件")]
        [StringLength(64, ErrorMessage = "必須在64個字以內")]
        public string? Email { get; set; }

        [Display(Name = "婚姻狀況")]
        public MarriageStatus? MarriageStatus { get; set; }

        [Display(Name = "在職狀況")]
        public JobStatus? JobStatus { get; set; } = 0;

        [Display(Name = "備註")]
        [StringLength(256, ErrorMessage = "必須在256個字以內")]
        public string? Momo { get; set; }
    }
}
