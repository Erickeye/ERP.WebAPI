using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class NewMirgration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_1000Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffUid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChineseName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    BankAccount = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OfficialEmail = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HighestEducation = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OnBoardDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResignationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCard = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    LaborPension = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    EmergencyContact1Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EmergencyContact1Relationship = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    EmergencyContact1Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EmergencyContact1Address = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EmergencyContact2Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EmergencyContact2Relationship = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    EmergencyContact2Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    EmergencyContact2Address = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    LineId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ExtensionNumber = table.Column<int>(type: "int", nullable: true),
                    BusinessPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Headshot = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SubBankName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    BloodType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1000Staff", x => x.StaffId);
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
                    f_ProjectStaff_LaborPension = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
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
                    f_PTarget_Achieve = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1020PerformanceTarget", x => x.f_PTarget_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1040Document",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Original = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Remark1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark2 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contract = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Authorizator = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Approval = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1040Document", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "t_1050WorkOver",
                columns: table => new
                {
                    WorkOverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Applicant = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    OvertimeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OvertimeType = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    OverTimeHours = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SignaturePath = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Authorizator = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1050WorkOver", x => x.WorkOverId);
                });

            migrationBuilder.CreateTable(
                name: "t_1080Company",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttribName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TaxInvoiceNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    TaxSerialNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    FaxPhone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    RegisteredAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    TaxInvoiceAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CheckingAccount = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    RemittanceAccount = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PayDays = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    FoundedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceForm = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1080Company", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1100Department",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1100Department", x => x.Id);
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
                    f_PettyCash_TotalAmount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
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
                    f_PettyCashDetail_Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    f_PettyCashDetail_Tax = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    f_PettyCashDetail_Total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
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

            migrationBuilder.CreateTable(
                name: "t_2000customer",
                columns: table => new
                {
                    f_customer_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_customer_UID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_customer_AttribName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_customer_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_customer_TaxInvoiceNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    f_customer_Owner = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_customer_ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_customer_FaxPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_staff_ChineseName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_customer_RegisteredAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    f_customer_DeliveryAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_customer_TaxInvoiceAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_customer_BankName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_customer_CheckingAccount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_customer_RemittanceAccount = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_customer_PayDays = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    f_customer_CreditLine = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    f_customer_CreditBalance = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    f_customer_LastDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    f_customer_Advance = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    f_custome_InvoiceForm = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_2000customer", x => x.f_customer_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_2010custemploy",
                columns: table => new
                {
                    f_custemploy_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_customer_ID = table.Column<int>(type: "int", nullable: false),
                    f_customer_AttribName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_customer_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_custemploy_Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    f_custemploy_Department = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_Post = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_Job = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_ExtNum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_MobilePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_Account = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    f_custemploy_Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    f_custemploy_EmotionState = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    f_custemploy_Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    f_custemploy_Momo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_2010custemploy", x => x.f_custemploy_ID);
                });

            migrationBuilder.CreateTable(
                name: "t_1001StaffCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_t_1001StaffCertificates_t_1000Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "t_1000Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_1030Dayoff",
                columns: table => new
                {
                    DayOffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    Applicant = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Proxy = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    LeaveType = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProxySignature = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Authorizer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalStatus = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1030Dayoff", x => x.DayOffId);
                    table.ForeignKey(
                        name: "FK_t_1030Dayoff_t_1000Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "t_1000Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_1101DepartmentUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsManager = table.Column<bool>(type: "bit", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_1101DepartmentUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_1101DepartmentUnit_t_1000Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "t_1000Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_1101DepartmentUnit_t_1100Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "t_1100Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_t_1039DayoffProxy_t_1030Dayoff_DayoffId",
                        column: x => x.DayoffId,
                        principalTable: "t_1030Dayoff",
                        principalColumn: "DayOffId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_1001StaffCertificates_StaffId",
                table: "t_1001StaffCertificates",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1030Dayoff_StaffId",
                table: "t_1030Dayoff",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1039DayoffProxy_DayoffId",
                table: "t_1039DayoffProxy",
                column: "DayoffId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1101DepartmentUnit_DepartmentId",
                table: "t_1101DepartmentUnit",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1101DepartmentUnit_StaffId",
                table: "t_1101DepartmentUnit",
                column: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_1001StaffCertificates");

            migrationBuilder.DropTable(
                name: "t_1005ProjectStaff");

            migrationBuilder.DropTable(
                name: "t_1020PerformanceTarget");

            migrationBuilder.DropTable(
                name: "t_1039DayoffProxy");

            migrationBuilder.DropTable(
                name: "t_1040Document");

            migrationBuilder.DropTable(
                name: "t_1050WorkOver");

            migrationBuilder.DropTable(
                name: "t_1080Company");

            migrationBuilder.DropTable(
                name: "t_1101DepartmentUnit");

            migrationBuilder.DropTable(
                name: "t_1200PettyCash");

            migrationBuilder.DropTable(
                name: "t_1201PettyCashDetail");

            migrationBuilder.DropTable(
                name: "t_1700LoginLog");

            migrationBuilder.DropTable(
                name: "t_1710ActionInfo");

            migrationBuilder.DropTable(
                name: "t_2000customer");

            migrationBuilder.DropTable(
                name: "t_2010custemploy");

            migrationBuilder.DropTable(
                name: "t_1030Dayoff");

            migrationBuilder.DropTable(
                name: "t_1100Department");

            migrationBuilder.DropTable(
                name: "t_1000Staff");
        }
    }
}
