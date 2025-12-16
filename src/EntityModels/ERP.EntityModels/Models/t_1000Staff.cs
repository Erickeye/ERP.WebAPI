using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1000Staff
{
    public int StaffId { get; set; }

    public string? StaffUid { get; set; }

    public string ChineseName { get; set; } = null!;

    public string? EnglishName { get; set; }

    public int Gender { get; set; }

    public string Account { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string BankAccount { get; set; } = null!;

    public string ContactAddress { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? OfficialEmail { get; set; }

    public DateTime Birthday { get; set; }

    public string HighestEducation { get; set; } = null!;

    public DateTime OnBoardDate { get; set; }

    public DateTime? ResignationDate { get; set; }

    public string IdCard { get; set; } = null!;

    public decimal LaborPension { get; set; }

    public string? EmergencyContact1Name { get; set; }

    public string? EmergencyContact1Relationship { get; set; }

    public string? EmergencyContact1Phone { get; set; }

    public string? EmergencyContact1Address { get; set; }

    public string? EmergencyContact2Name { get; set; }

    public string? EmergencyContact2Relationship { get; set; }

    public string? EmergencyContact2Phone { get; set; }

    public string? EmergencyContact2Address { get; set; }

    public string? LineId { get; set; }

    public int? ExtensionNumber { get; set; }

    public string? BusinessPhone { get; set; }

    public byte[]? Headshot { get; set; }

    public string? BankName { get; set; }

    public string? SubBankName { get; set; }

    public int BloodType { get; set; }

    public virtual ICollection<t_1001StaffCertificates> t_1001StaffCertificates { get; set; } = new List<t_1001StaffCertificates>();

    public virtual ICollection<t_1030Dayoff> t_1030DayoffApplicantNavigation { get; set; } = new List<t_1030Dayoff>();

    public virtual ICollection<t_1030Dayoff> t_1030DayoffLeaveTakerNavigation { get; set; } = new List<t_1030Dayoff>();

    public virtual ICollection<t_1030Dayoff> t_1030DayoffProxyNavigation { get; set; } = new List<t_1030Dayoff>();

    public virtual ICollection<t_1101DepartmentUnit> t_1101DepartmentUnit { get; set; } = new List<t_1101DepartmentUnit>();
}
