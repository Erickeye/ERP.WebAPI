using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1030Dayoff
    {
        [Key]
        [Display(Name = "請假單編號")]
        public int? f_Dayoff_ID { get; set; }

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_Dayoff_Date { get; set; }

        [Display(Name = "請假人")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Dayoff_Name { get; set; }

        [Display(Name = "申請者")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Dayoff_Applicant { get; set; }

        [Display(Name = "部門")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Dayoff_Department { get; set; }

        [Display(Name = "代理人")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Dayoff_Position { get; set; }

        [Display(Name = "假別")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Dayoff_LeaveType { get; set; }

        [Display(Name = "事由")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_Dayoff_Reason { get; set; }

        [Display(Name = "代理人簽章")]
        public string? f_Dayoff_ProxySign { get; set; }

        [Display(Name = "開始日期")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "必填欄位")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public System.DateTime? f_Dayoff_BeginDate { get; set; }

        [Display(Name = "結束日期")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "必填欄位")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public System.DateTime? f_Dayoff_EndDate { get; set; }

        [Display(Name = "簽核主管")]
        public string? f_Dayoff_Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? f_Dayoff_Approval { get; set; }
    }
}
