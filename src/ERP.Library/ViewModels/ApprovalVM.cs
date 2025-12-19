using System;
using System.ComponentModel.DataAnnotations;
using ERP.Library.Enums.Other;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels
{
    public class SendApprovalProcessVM
    {
        [SwaggerParameter("表格種類")]
        public TableType TableType { get; set; }

        [SwaggerParameter("表格Id")]
        public string TableId { get; set; } = null!;
    }
    public class ApprovalVM
    {
        public TableType TableType { get; set; }
        public string? TableId { get; set; } = null!;
        public string? Memo { get; set; } = null!;
    }
    public class RejectApprovalVM
    {
        public TableType TableType { get; set; }
        public string? TableId { get; set; } = null!;
        public string? Memo { get; set; } = null!;
    }

    //============================= 【1.簽核模組】=============================
    /// <summary>
    /// 設定【1.簽核模組】
    /// </summary>
    public class ApprovalSettingsInputVM
    {
        [SwaggerSchema("關聯Id")]
        public int Id { get; set; }

        [SwaggerSchema("簽核表單種類")]
        public int TableType { get; set; }

        [StringLength(64, ErrorMessage = "【簽核模組名稱】長度不可超過 64 個字元")]
        [SwaggerSchema("簽核模組名稱")]
        public string? Name { get; set; } = null!;

        [SwaggerSchema("是否啟用")]
        public bool IsActive { get; set; }

    }

    /// <summary>
    /// 設定【1.簽核模組】
    /// </summary>
    public class ApprovalCheckSettingsVM
    {
        [SwaggerSchema("關聯Id")]
        public int Id { get; set; }

        [SwaggerSchema("簽核表單種類")]
        public int TableType { get; set; }

        [SwaggerSchema("簽核表單種類顯示")]
        public string? TableTypeDisplay { get; set; }

        [StringLength(64, ErrorMessage = "【簽核模組名稱】長度不可超過 64 個字元")]
        [SwaggerSchema("簽核模組名稱")]
        public string? Name { get; set; } = null!;

        [SwaggerSchema("是否啟用")]
        public bool IsActive { get; set; }

        public List<ApprovalStepVM> Steps { get; set; } =
            new List<ApprovalStepVM>();
    }
    public class GetApprovalProgressVM
    {
        [SwaggerSchema("簽核紀錄流水號")]
        public int Id { get; set; }

        [SwaggerSchema("順序")]
        public int StepOrder { get; set; }

        [SwaggerSchema("簽核表單種類")]
        public int TableType { get; set; }

        [SwaggerSchema("表格名稱")]
        public string? TableName { get; set; }

        [SwaggerSchema("表格Id")]
        public string? TableId{ get; set; }

        [SwaggerSchema("使用者Id")]
        public int? UserId { get; set; }

        [SwaggerSchema("使用者名稱")]
        public string? UserName { get; set; }

        [SwaggerSchema("角色Id")]
        public int? RoleId { get; set; }

        [SwaggerSchema("角色名稱")]
        public string? RoleName { get; set; }

        [SwaggerSchema("狀態")]
        public int Status { get; set; }

        [SwaggerSchema("狀態(顯示)")]
        public string? StatusDisplay { get; set; }

        [SwaggerSchema("備註")]
        public string? Memo { get; set; }
    }

    //============================= 【2.簽核步驟】=============================
    /// <summary>
    /// 檢視【2.簽核步驟】
    /// </summary>
    public class ApprovalStepVM
    {
        [SwaggerSchema("簽核步驟Id")]
        public int Id { get; set; }

        [SwaggerSchema("模式順序")]
        public int StepOrder { get; set; }

        [SwaggerSchema("腳色Id")]
        public int RoleId { get; set; }

        [SwaggerSchema("簽核模式模型")]
        public int Mode { get; set; }

        [SwaggerSchema("簽核模式模型顯示")]
        public string? ModeDisplay { get; set; }

        [SwaggerSchema("需求數量")]
        public int? RequiredCounts { get; set; }

        public List<ApprovalStepNumberVM> StepNumbers { get; set; } =
           new List<ApprovalStepNumberVM>();
    }
    /// <summary>
    /// 檢視【3.簽核步驟成員】
    /// </summary>
    public class ApprovalStepNumberVM
    {
        [SwaggerSchema("簽核步驟成員Id")]
        public int Id { get; set; }

        [SwaggerSchema("簽核步驟Id")]
        public int ApprovalStepId { get; set; }

        [SwaggerSchema("使用者Id")]
        public int UserId { get; set; }
    }

    /// <summary>
    /// 設定【2.簽核步驟】
    /// </summary>
    public class ApprovalStepInputVM
    {
        [SwaggerSchema("簽核步驟Id")]
        public int Id { get; set; }

        [SwaggerSchema("簽核模組Id")]
        public int ApprovalSettingsId { get; set; }

        [SwaggerSchema("簽核角色")]
        public int RoleId { get; set; }

        [SwaggerSchema("簽核模式")]
        public int Mode { get; set; } // 指定人員 / 單人通過 / 自訂人數

        [SwaggerSchema("自訂人數")]
        public int RequiredCounts { get; set; }

    }

    public class ApprovalStepNumberInputVM
    {
        public int Id { get; set; }
        public int ApprovalStepId { get; set; }

        [Display(Name = "使用者Id")]
        public int UserId { get; set; }
    }
}
