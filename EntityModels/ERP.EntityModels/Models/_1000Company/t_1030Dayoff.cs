using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Library.Enums._1000Company;
using ERP.Library.Enums.Other;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1030Dayoff
    {
        [Key]
        [Display(Name = "請假單編號")]
        public int Id { get; set; }

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApplicationDate { get; set; }

        [ForeignKey(nameof(Staff_LeaveTaker))]
        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "請假人")]
        public int LeaveTaker { get; set; }

        [ForeignKey(nameof(Staff_Applicant))]
        [Display(Name = "申請者")]
        public int? Applicant { get; set; }

        [ForeignKey(nameof(Staff_Proxy))]
        [Display(Name = "代理人")]        
        public int? Proxy { get; set; }

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

        [Display(Name = "簽核狀態")]
        public ApprovalStatus? ApprovalStatus { get; set; }

        public virtual t_1000Staff? Staff_LeaveTaker { get; set; }
        public virtual t_1000Staff? Staff_Applicant { get; set; }
        public virtual t_1000Staff? Staff_Proxy { get; set; }
    }
}
