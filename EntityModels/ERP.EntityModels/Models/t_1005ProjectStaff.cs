using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1005ProjectStaff
{
    public int f_ProjectStaff_ID { get; set; }

    public string? f_ProjectStaff_UID { get; set; }

    public string f_ProjectStaff_ChineseName { get; set; } = null!;

    public string? f_ProjectStaff_EnglishName { get; set; }

    public string f_ProjectStaff_Gender { get; set; } = null!;

    public string? f_ProjectStaff_Account { get; set; }

    public string? f_ProjectStaff_ContactPhone { get; set; }

    public string f_ProjectStaff_BankName { get; set; } = null!;

    public string? f_ProjectStaff_SubBankName { get; set; }

    public string f_ProjectStaff_BankAccount { get; set; } = null!;

    public string f_ProjectStaff_ContactAddress { get; set; } = null!;

    public string? f_ProjectStaff_Email { get; set; }

    public DateTime f_ProjectStaff_Bitrthday { get; set; }

    public string f_ProjectStaff_HighestEducation { get; set; } = null!;

    public DateTime? f_ProjectStaff_OnBoardDay { get; set; }

    public DateTime? f_ProjectStaff_ResignationDay { get; set; }

    public string f_ProjectStaff_IDCard { get; set; } = null!;

    public decimal? f_ProjectStaff_LaborPension { get; set; }

    public string? f_ProjectStaff_EC1Name { get; set; }

    public string? f_ProjectStaff_EC1Relationship { get; set; }

    public string? f_ProjectStaff_EC1Cellphone { get; set; }

    public string? f_ProjectStaff_EC1Address { get; set; }

    public string? f_ProjectStaff_LineID { get; set; }

    public string? f_ProjectStaff_ContractID { get; set; }
}
