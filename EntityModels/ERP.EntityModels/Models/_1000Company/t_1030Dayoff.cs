using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERP.Library.Enums._1000Company;

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

        [ForeignKey(nameof(Staff))]
        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "請假人")]
        public int StaffId { get; set; }

        [Display(Name = "申請者")]
        [StringLength(32)]
        public string? Applicant { get; set; }

        [Display(Name = "部門")]
        [StringLength(32)]
        public string? Department { get; set; }

        [Display(Name = "代理人")]
        [StringLength(32)]
        public string? Proxy { get; set; }

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
        public string? Authorizer { get; set; }

        [Display(Name = "簽核")]
        public bool? ApprovalStatus { get; set; }

        public virtual t_1000Staff? Staff { get; set; }
    }
}
