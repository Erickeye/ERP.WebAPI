using ERP.Library.Enums._1000Company;
using System.ComponentModel.DataAnnotations;


namespace ERP.EntityModels.Models._1000Company
{
    public class t_1040Document
    {
        [Key]
        [Display(Name = "發文字號")]
        public int Id { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "公司名稱")]
        [StringLength(64, ErrorMessage = "必須在 64 位數以內")]
        public string? Company { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "聯絡人")]
        [StringLength(32, ErrorMessage = "必須在 32 位數以內")]
        public string? ContactPerson { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "受文者")]
        [StringLength(64, ErrorMessage = "必須在 64 位數以內")]
        public string? Recipient { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "發文日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DocumentDate { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "速別")]
        public DocumentLevelType Level { get; set; }

        [Display(Name = "附件")]
        [StringLength(32, ErrorMessage = "必須在 32 位數以內")]
        public string? Attachment { get; set; }

        [Display(Name = "主旨")]
        [StringLength(256, ErrorMessage = "必須在 256 位數以內")]
        public string? Subject { get; set; }

        [Display(Name = "正本")]
        [StringLength(64, ErrorMessage = "必須在 64 位數以內")]
        public string? Original { get; set; }

        [Display(Name = "說明")]
        public string? Remark1 { get; set; }

        [Display(Name = "副本")]
        [StringLength(64, ErrorMessage = "必須在 64 位數以內")]
        public string? Remark2 { get; set; }

        [Display(Name = "圖片")]
        public string? File { get; set; }

        [Display(Name = "專案編號")]
        [StringLength(32, ErrorMessage = "必須在 32 位數以內")]
        public string? Contract { get; set; }

        [Display(Name = "主管簽核")]
        [StringLength(32, ErrorMessage = "必須在 32 位數以內")]
        public string? Authorizator { get; set; }

        [Display(Name = "簽核")]
        public bool? Approval { get; set; }
    }

}
