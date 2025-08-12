using ERP.Library.Enums._1000Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class DayOffListVM
    {
        [Display(Name = "請假人")]
        public string LeaveTaker { get; set; } = null!;

        [Display(Name = "申請日期")]
        public DateTime? ApplicationDate { get; set; }

        [Display(Name = "代理人")]
        public string? Proxy { get; set; }

        [Display(Name = "假別")]
        public LeaveType LeaveType { get; set; }

        [Display(Name = "事由")]
        public string? Reason { get; set; }

        [Display(Name = "開始日期")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "結束日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "簽核主管")]
        public string? Authorizer { get; set; }
    }
    public class DayOffInputVM
    {
        /// <summary>請假單編號</summary>
        public int? Id { get; set; }  // 修改時需要，新增時可不傳或 null

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApplicationDate { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "請假人")]
        public int LeaveTaker { get; set; }

        [Display(Name = "申請者")]
        public int? Applicant { get; set; }

        [Display(Name = "代理人")]
        public int? Proxy { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "假別")]
        public LeaveType LeaveType { get; set; }

        [Display(Name = "事由")]
        [StringLength(64)]
        public string? Reason { get; set; }

        [Display(Name = "代理人簽章")]
        [StringLength(128)]
        public string? ProxySignature { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "開始日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime BeginDate { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "結束日期")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "簽核主管")]
        [StringLength(64)]
        public string? Authorizer { get; set; }

        [Display(Name = "簽核")]
        public bool? ApprovalStatus { get; set; }
    }
}
