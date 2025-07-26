using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1005ProjectStaff
    {
        [Key]
        [Display(Name = "流水號")]

        public int f_ProjectStaff_ID { get; set; }

        [Display(Name = "員工編號")]
        public string? f_ProjectStaff_UID { get; set; }
        [Display(Name = "員工中文名 ")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_ChineseName { get; set; }
        [Display(Name = "員工英文名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_EnglishName { get; set; }
        [Display(Name = "性別")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_Gender { get; set; }
        [Display(Name = "使用者帳號")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_Account { get; set; }
        [Display(Name = "連絡電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_ContactPhone { get; set; }
        [Display(Name = "銀行名稱")]
        [StringLength(30, ErrorMessage = "必須在30位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_BankName { get; set; }
        [Display(Name = "分行別")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_SubBankName { get; set; }

        [Display(Name = "銀行帳號")]
        [StringLength(30, ErrorMessage = "必須在30位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_BankAccount { get; set; }
        [Display(Name = "通訊地址")]
        [Required(ErrorMessage = "必填欄位")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_ProjectStaff_ContactAddress { get; set; }
        [Display(Name = "電子郵件")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_ProjectStaff_Email { get; set; }
        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "必填欄位")]
        public System.DateTime? f_ProjectStaff_Bitrthday { get; set; }
        [Display(Name = "最高學歷")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_HighestEducation { get; set; }
        [Display(Name = "到職日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_ProjectStaff_OnBoardDay { get; set; }
        [Display(Name = "離職日")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime? f_ProjectStaff_ResignationDay { get; set; }
        [Display(Name = "身分證字號")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        [Required(ErrorMessage = "必填欄位")]
        public string? f_ProjectStaff_IDCard { get; set; }
        [Display(Name = "員工自行提撥勞退")]
        public decimal? f_ProjectStaff_LaborPension { get; set; }
        [Display(Name = "緊急連絡人1姓名")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_EC1Name { get; set; }
        [Display(Name = "緊急連絡人1關係")]
        [StringLength(10, ErrorMessage = "必須在10位數以內")]
        public string? f_ProjectStaff_EC1Relationship { get; set; }
        [Display(Name = "緊急連絡人1電話")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_EC1Cellphone { get; set; }
        [Display(Name = "緊急連絡人1住址")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_ProjectStaff_EC1Address { get; set; }

        [Display(Name = "LineID")]
        [StringLength(50, ErrorMessage = "必須在50位數以內")]
        public string? f_ProjectStaff_LineID { get; set; }

        [Display(Name = "專案編號")]
        [StringLength(20, ErrorMessage = "必須在20位數以內")]
        public string? f_ProjectStaff_ContractID { get; set; }
    }
}
