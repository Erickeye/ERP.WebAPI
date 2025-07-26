using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1050WorkOver
    {
        [Key]
        [Display(Name = "流水號")]
        public int f_WorkOver_ID { get; set; }

        [Display(Name = "申請人")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_WorkOver_Applicant { get; set; }

        [Display(Name = "部門")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_WorkOver_Department { get; set; }

        [Display(Name = "職稱")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_WorkOver_JobTitle { get; set; }

        [Display(Name = "加班日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_WorkOver_OvertimeDate { get; set; }

        [Display(Name = "加班類別")]
        public string? f_WorkOver_OvertimeType { get; set; }

        [Display(Name = "開始")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan? f_WorkOver_StartTime { get; set; }

        [Display(Name = "結束")]
        [Required(ErrorMessage = "必填欄位")]
        [DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm:ss}")]
        public TimeSpan? f_WorkOver_EndTime { get; set; }

        [Display(Name = "加班時數")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal? f_WorkOver_Time { get; set; }

        [Display(Name = "事由")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_WorkOver_Reason { get; set; }

        [Display(Name = "申請人簽名")]
        public string? f_WorkOver_Signature { get; set; }

        [Display(Name = "簽核主管")]
        public string? f_WorkOver_Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? f_WorkOver_Approval { get; set; }
    }
}
