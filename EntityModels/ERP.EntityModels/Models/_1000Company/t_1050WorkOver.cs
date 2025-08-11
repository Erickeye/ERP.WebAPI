using ERP.Library.Enums._1000Company;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1050WorkOver
    {
        [Key]
        [Display(Name = "流水號")]
        public int WorkOverId { get; set; }

        [Display(Name = "申請人")]
        [StringLength(32, ErrorMessage = "必須在32位字以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? Applicant { get; set; }

        [Display(Name = "部門")]
        [StringLength(32, ErrorMessage = "必須在32位字以內")]
        public string? Department { get; set; }

        [Display(Name = "職稱")]
        [StringLength(32, ErrorMessage = "必須在32位字以內")]
        public string? JobTitle { get; set; }

        [Display(Name = "加班日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? OvertimeDate { get; set; }

        [Display(Name = "加班類別")]
        [Required(ErrorMessage = "必填欄位")]
        public OverTimeType OvertimeType { get; set; }

        [Display(Name = "開始時間")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "結束時間")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        [Display(Name = "加班時數")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal? OverTimeHours { get; set; }

        [Display(Name = "加班事由")]
        [StringLength(64, ErrorMessage = "必須在64位字以內")]
        public string? Reason { get; set; }

        [Display(Name = "申請人簽名")]
        [StringLength(64)]
        public string? SignaturePath { get; set; }

        [Display(Name = "簽核主管")]
        [StringLength(32)]
        public string? Authorizator { get; set; }

        [Display(Name = "是否核准")]
        public bool? IsApproved { get; set; }
    }
}
