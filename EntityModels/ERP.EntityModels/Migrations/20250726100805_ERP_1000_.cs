using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class ERP_1000_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_1000Staff",
                columns: table => new
                {
                    f_staff_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_staff_UID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_staff_ChineseName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_staff_EnglishName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_staff_Account = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_staff_ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_staff_BankAccount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_staff_ContactAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    f_staff_Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    f_staff_OfficialMail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_staff_Bitrthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_staff_HighestEducation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_staff_OnBoardDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_staff_ResignationDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_staff_IDCard = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    f_staff_LaborPension = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    f_staff_EC1Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_EC1Relationship = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    f_staff_EC1Cellphone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_EC1Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_staff_EC2Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_EC2Relationship = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    f_staff_EC2Cellphone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_EC2Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_staff_LineID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_staff_ExtensionNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_staff_BusinessPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_Headshot = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    f_staff_BankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_staff_SubBankName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_BloodType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1000Staff", x => x.f_staff_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1001StaffCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: true),
                    Certificate = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CertificateName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CertificateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveDate = table.Column<int>(type: "int", nullable: true),
                    IsNotify = table.Column<bool>(type: "bit", nullable: true),
                    NotifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1001StaffCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_1005ProjectStaff",
                columns: table => new
                {
                    f_ProjectStaff_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_ProjectStaff_UID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ProjectStaff_ChineseName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_ProjectStaff_EnglishName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_ProjectStaff_Account = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_BankName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    f_ProjectStaff_SubBankName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_BankAccount = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    f_ProjectStaff_ContactAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    f_ProjectStaff_Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_ProjectStaff_Bitrthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_ProjectStaff_HighestEducation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_ProjectStaff_OnBoardDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_ProjectStaff_ResignationDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_ProjectStaff_IDCard = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    f_ProjectStaff_LaborPension = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_ProjectStaff_EC1Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_EC1Relationship = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    f_ProjectStaff_EC1Cellphone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_ProjectStaff_EC1Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_ProjectStaff_LineID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_ProjectStaff_ContractID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1005ProjectStaff", x => x.f_ProjectStaff_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1020PerformanceTarget",
                columns: table => new
                {
                    f_PTarget_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_staff_UID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_staff_ChineseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_PTarget_Annyal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_PTarget_Achieve = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1020PerformanceTarget", x => x.f_PTarget_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1030Dayoff",
                columns: table => new
                {
                    f_Dayoff_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_Dayoff_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_Dayoff_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_Dayoff_Applicant = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Dayoff_Department = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Dayoff_Position = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Dayoff_LeaveType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Dayoff_Reason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Dayoff_ProxySign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_Dayoff_BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_Dayoff_EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_Dayoff_Authorizator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_Dayoff_Approval = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1030Dayoff", x => x.f_Dayoff_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1039DayoffProxy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayoffId = table.Column<int>(type: "int", nullable: false),
                    ProxyId = table.Column<int>(type: "int", nullable: false),
                    ProxyAgree = table.Column<bool>(type: "bit", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SelfId = table.Column<int>(type: "int", nullable: false),
                    IsConfirm = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1039DayoffProxy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_1040Document",
                columns: table => new
                {
                    f_Document_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_Document_Company = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Document_ContactPerson = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Document_Recipient = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Document_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_Document_Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Document_Attachment = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Document_Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    f_Document_Original = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Document_Remark1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_Document_Remark2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Document_File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_Document_Contract = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_Document_Authorizator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_Document_Approval = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1040Document", x => x.f_Document_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1050WorkOver",
                columns: table => new
                {
                    f_WorkOver_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_WorkOver_Applicant = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_WorkOver_Department = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_WorkOver_JobTitle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_WorkOver_OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_WorkOver_OvertimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_WorkOver_StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    f_WorkOver_EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    f_WorkOver_Time = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_WorkOver_Reason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_WorkOver_Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_WorkOver_Authorizator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_WorkOver_Approval = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1050WorkOver", x => x.f_WorkOver_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1100Department",
                columns: table => new
                {
                    f_deprtmt_ID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    f_deprtmt_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1100Department", x => x.f_deprtmt_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1101Deprtmt",
                columns: table => new
                {
                    f_staff_UID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    f_deprtmt_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_deprtmt_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_staff_ChineseName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_deprtmt_MG = table.Column<bool>(type: "bit", nullable: false),
                    f_deprtmt_Seniority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1101Deprtmt", x => x.f_staff_UID);
                });

            migrationBuilder.CreateTable(
                name: "t_1200PettyCash",
                columns: table => new
                {
                    f_PettyCash_ID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_PettyCash_Payee = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_PettyCash_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_PettyCash_Company = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_PettyCash_Reason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_PettyCash_TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_PettyCash_Handler = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_Supervisor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_Manager = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_Accountant = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_PettyCash_Filler = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_Authorizator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_PettyCash_Approval = table.Column<bool>(type: "bit", nullable: true),
                    f_PettyCash_Accounting = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1200PettyCash", x => x.f_PettyCash_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1201PettyCashDetail",
                columns: table => new
                {
                    f_PettyCashDetail_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_PettyCash_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_PettyCashDetail_Project = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_PurchaseID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_Vehicle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_Supplier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_PettyCashDetail_InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_PettyCashDetail_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_PettyCashDetail_Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_PettyCashDetail_Total = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    f_PettyCashDetail_Sort = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1201PettyCashDetail", x => x.f_PettyCashDetail_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1700LoginLog",
                columns: table => new
                {
                    f_login_UID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_login_CrateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_staff_Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_login_Type = table.Column<byte>(type: "tinyint", nullable: true),
                    f_login_IP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1700LoginLog", x => x.f_login_UID);
                });

            migrationBuilder.CreateTable(
                name: "t_1710ActionInfo",
                columns: table => new
                {
                    f_ActionInfo_UID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_ActionInfo_Account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ActionInfo_IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ActionInfo_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_ActionInfo_Controller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ActionInfo_Function = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ActionInfo_ControllerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_ActionInfo_describe = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1710ActionInfo", x => x.f_ActionInfo_UID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_1000Staff");

            migrationBuilder.DropTable(
                name: "t_1001StaffCertificates");

            migrationBuilder.DropTable(
                name: "t_1005ProjectStaff");

            migrationBuilder.DropTable(
                name: "t_1020PerformanceTarget");

            migrationBuilder.DropTable(
                name: "t_1030Dayoff");

            migrationBuilder.DropTable(
                name: "t_1039DayoffProxy");

            migrationBuilder.DropTable(
                name: "t_1040Document");

            migrationBuilder.DropTable(
                name: "t_1050WorkOver");

            migrationBuilder.DropTable(
                name: "t_1100Department");

            migrationBuilder.DropTable(
                name: "t_1101Deprtmt");

            migrationBuilder.DropTable(
                name: "t_1200PettyCash");

            migrationBuilder.DropTable(
                name: "t_1201PettyCashDetail");

            migrationBuilder.DropTable(
                name: "t_1700LoginLog");

            migrationBuilder.DropTable(
                name: "t_1710ActionInfo");
        }
    }
}
