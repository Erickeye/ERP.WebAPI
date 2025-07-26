using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1040Document
    {
        [Key]
        [Display(Name = "發文字號")]
        public int? f_Document_ID { get; set; }

        [Display(Name = "公司名稱")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_Document_Company { get; set; }

        [Display(Name = "聯絡人")]
        [StringLength(20, ErrorMessage = "必須在50位數以內")]
        public string? f_Document_ContactPerson { get; set; }

        [Display(Name = "受文者")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_Document_Recipient { get; set; }

        [Display(Name = "發文日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_Document_Date { get; set; }

        [Display(Name = "速別")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Document_Level { get; set; }

        [Display(Name = "附件")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Document_Attachment { get; set; }

        [Display(Name = "主旨")]
        [StringLength(200, ErrorMessage = "必須在200位數以內")]
        public string? f_Document_Subject { get; set; }

        [Display(Name = "正本")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_Document_Original { get; set; }

        [Display(Name = "說明")]
        public string? f_Document_Remark1 { get; set; }

        [Display(Name = "副本")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_Document_Remark2 { get; set; }

        [Display(Name = "圖片")]
        public string? f_Document_File { get; set; }

        [Display(Name = "專案編號")]
        [StringLength(50, ErrorMessage = "必須在20位數以內")]
        public string? f_Document_Contract { get; set; }

        [Display(Name = "主管簽核")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_Document_Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? f_Document_Approval { get; set; }
    }
}
