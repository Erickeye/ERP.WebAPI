using System.ComponentModel.DataAnnotations;

namespace ERP.EntityModels.Models._1000Company
{
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public enum BloodType
    {
        Unknown = 0,
        A = 1,
        B = 2,
        AB = 3,
        O = 4
    }

    public class t_1000Staff
    {
        [Key]
        [Display(Name = "流水號")]
        public int StaffId { get; set; }

        [Display(Name = "員工編號")]
        public string? StaffUid { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "員工中文名")]
        public string? ChineseName { get; set; }

        [StringLength(32)]
        [Display(Name = "員工英文名")]
        public string? EnglishName { get; set; }

        [Required]
        [Display(Name = "性別")]
        public Gender Gender { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "使用者帳號")]
        public string? Account { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "連絡電話")]
        public string? ContactPhone { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "銀行帳號")]
        public string? BankAccount { get; set; }

        [Required]
        [StringLength(64)]
        [Display(Name = "通訊地址")]
        public string? ContactAddress { get; set; }

        [Required]
        [StringLength(64)]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string? Email { get; set; }

        [StringLength(64)]
        [EmailAddress]
        [Display(Name = "公務郵件")]
        public string? OfficialEmail { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        [Required]
        [StringLength(32)]
        [Display(Name = "最高學歷")]
        public string? HighestEducation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "到職日")]
        public DateTime? OnBoardDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "離職日")]
        public DateTime? ResignationDate { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name = "身分證字號")]
        public string? IdCard { get; set; }

        [Required]
        [Range(0, 999999999999)]
        [Display(Name = "員工自行提撥勞退")]
        public decimal? LaborPension { get; set; }

        [StringLength(32)]
        [Display(Name = "緊急連絡人1姓名")]
        public string? EmergencyContact1Name { get; set; }

        [StringLength(16)]
        [Display(Name = "緊急連絡人1關係")]
        public string? EmergencyContact1Relationship { get; set; }

        [StringLength(32)]
        [Display(Name = "緊急連絡人1電話")]
        public string? EmergencyContact1Phone { get; set; }

        [StringLength(64)]
        [Display(Name = "緊急連絡人1住址")]
        public string? EmergencyContact1Address { get; set; }

        [StringLength(32)]
        [Display(Name = "緊急連絡人2姓名")]
        public string? EmergencyContact2Name { get; set; }

        [StringLength(16)]
        [Display(Name = "緊急連絡人2關係")]
        public string? EmergencyContact2Relationship { get; set; }

        [StringLength(32)]
        [Display(Name = "緊急連絡人2電話")]
        public string? EmergencyContact2Phone { get; set; }

        [StringLength(64)]
        [Display(Name = "緊急連絡人2住址")]
        public string? EmergencyContact2Address { get; set; }

        [StringLength(64)]
        [Display(Name = "Line ID")]
        public string? LineId { get; set; }

        [Display(Name = "分機號碼")]
        public int? ExtensionNumber { get; set; }

        [StringLength(32)]
        [Display(Name = "公務手機")]
        public string? BusinessPhone { get; set; }

        [Display(Name = "大頭照")]
        public byte[]? Headshot { get; set; }

        [StringLength(64)]
        [Display(Name = "銀行名稱")]
        public string? BankName { get; set; }

        [StringLength(32)]
        [Display(Name = "分行別")]
        public string? SubBankName { get; set; }

        [Display(Name = "血型")]
        public BloodType BloodType { get; set; }

        public virtual ICollection<t_1001StaffCertificates> StaffCertificates { get; set; } = new List<t_1001StaffCertificates>();
    }

}
