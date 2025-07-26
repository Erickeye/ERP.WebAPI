using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1000Staff
    {
        [Key]
        [Display(Name = "流水號")]
        public int f_staff_ID { get; set; }

        [Display(Name = "員工編號")]
        public string? f_staff_UID { get; set; }

        [Display(Name = "員工中文名 ")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_ChineseName { get; set; }

        [Display(Name = "員工英文名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_EnglishName { get; set; }

        [Display(Name = "性別")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_Gender { get; set; }

        [Display(Name = "使用者帳號")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_Account { get; set; }

        [Display(Name = "連絡電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_ContactPhone { get; set; }

        [Display(Name = "銀行帳號")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_BankAccount { get; set; }

        [Display(Name = "通訊地址")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_ContactAddress { get; set; }

        [Display(Name = "電子郵件")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_Email { get; set; }

        [Display(Name = "公務郵件")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_OfficialMail { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "必填欄位")]
        public System.DateTime? f_staff_Bitrthday { get; set; }

        [Display(Name = "最高學歷")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_HighestEducation { get; set; }

        [Display(Name = "到職日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "必填欄位")]
        public System.DateTime? f_staff_OnBoardDay { get; set; }

        [Display(Name = "離職日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_staff_ResignationDay { get; set; }

        [Display(Name = "身分證字號")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_staff_IDCard { get; set; }

        [Display(Name = "員工自行提撥勞退")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 999999999999, ErrorMessage = "必須在12位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public decimal? f_staff_LaborPension { get; set; }

        [Display(Name = "緊急連絡人1姓名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_EC1Name { get; set; }

        [Display(Name = "緊急連絡人1關係")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        public string? f_staff_EC1Relationship { get; set; }

        [Display(Name = "緊急連絡人1電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_EC1Cellphone { get; set; }

        [Display(Name = "緊急連絡人1住址")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_EC1Address { get; set; }

        [Display(Name = "緊急連絡人2姓名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_EC2Name { get; set; }

        [Display(Name = "緊急連絡人2關係")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        public string? f_staff_EC2Relationship { get; set; }

        [Display(Name = "緊急連絡人2電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_EC2Cellphone { get; set; }

        [Display(Name = "緊急連絡人2住址")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_EC2Address { get; set; }

        [Display(Name = "LineID")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_LineID { get; set; }

        [Display(Name = "分機號碼")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal? f_staff_ExtensionNumber { get; set; }

        [Display(Name = "公務手機")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_BusinessPhone { get; set; }

        [Display(Name = "大頭照")]
        public byte[]? f_staff_Headshot { get; set; }

        [Display(Name = "銀行名稱")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_staff_BankName { get; set; }

        [Display(Name = "分行別")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_staff_SubBankName { get; set; }

        [Display(Name = "血型")]
        public string? f_staff_BloodType { get; set; }
    }
}
