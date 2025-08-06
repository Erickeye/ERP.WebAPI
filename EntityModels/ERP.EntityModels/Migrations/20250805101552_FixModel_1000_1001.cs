using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class FixModel_1000_1001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "f_staff_Account",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_BankAccount",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_BankName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_BloodType",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_BusinessPhone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_ChineseName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_ContactAddress",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_ContactPhone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC1Address",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC1Cellphone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC1Name",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC1Relationship",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC2Address",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC2Cellphone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC2Name",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EC2Relationship",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_Email",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_EnglishName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_ExtensionNumber",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_Gender",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_HighestEducation",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_IDCard",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_LineID",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_OfficialMail",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "f_staff_SubBankName",
                table: "t_1000Staff");

            migrationBuilder.RenameColumn(
                name: "f_staff_UID",
                table: "t_1000Staff",
                newName: "StaffUid");

            migrationBuilder.RenameColumn(
                name: "f_staff_ResignationDay",
                table: "t_1000Staff",
                newName: "ResignationDate");

            migrationBuilder.RenameColumn(
                name: "f_staff_OnBoardDay",
                table: "t_1000Staff",
                newName: "OnBoardDate");

            migrationBuilder.RenameColumn(
                name: "f_staff_LaborPension",
                table: "t_1000Staff",
                newName: "LaborPension");

            migrationBuilder.RenameColumn(
                name: "f_staff_Headshot",
                table: "t_1000Staff",
                newName: "Headshot");

            migrationBuilder.RenameColumn(
                name: "f_staff_Bitrthday",
                table: "t_1000Staff",
                newName: "Birthday");

            migrationBuilder.RenameColumn(
                name: "f_staff_ID",
                table: "t_1000Staff",
                newName: "StaffId");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "t_1001StaffCertificates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloodType",
                table: "t_1000Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPhone",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChineseName",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactAddress",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1Address",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1Name",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1Phone",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact1Relationship",
                table: "t_1000Staff",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2Address",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2Name",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2Phone",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact2Relationship",
                table: "t_1000Staff",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtensionNumber",
                table: "t_1000Staff",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "t_1000Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HighestEducation",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdCard",
                table: "t_1000Staff",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LineId",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialEmail",
                table: "t_1000Staff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubBankName",
                table: "t_1000Staff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_1001StaffCertificates_StaffId",
                table: "t_1001StaffCertificates",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_1001StaffCertificates_t_1000Staff_StaffId",
                table: "t_1001StaffCertificates",
                column: "StaffId",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_1001StaffCertificates_t_1000Staff_StaffId",
                table: "t_1001StaffCertificates");

            migrationBuilder.DropIndex(
                name: "IX_t_1001StaffCertificates_StaffId",
                table: "t_1001StaffCertificates");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "BusinessPhone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "ChineseName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "ContactAddress",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact1Address",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact1Name",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact1Phone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact1Relationship",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2Address",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2Name",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2Phone",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EmergencyContact2Relationship",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "ExtensionNumber",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "HighestEducation",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "IdCard",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "LineId",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "OfficialEmail",
                table: "t_1000Staff");

            migrationBuilder.DropColumn(
                name: "SubBankName",
                table: "t_1000Staff");

            migrationBuilder.RenameColumn(
                name: "StaffUid",
                table: "t_1000Staff",
                newName: "f_staff_UID");

            migrationBuilder.RenameColumn(
                name: "ResignationDate",
                table: "t_1000Staff",
                newName: "f_staff_ResignationDay");

            migrationBuilder.RenameColumn(
                name: "OnBoardDate",
                table: "t_1000Staff",
                newName: "f_staff_OnBoardDay");

            migrationBuilder.RenameColumn(
                name: "LaborPension",
                table: "t_1000Staff",
                newName: "f_staff_LaborPension");

            migrationBuilder.RenameColumn(
                name: "Headshot",
                table: "t_1000Staff",
                newName: "f_staff_Headshot");

            migrationBuilder.RenameColumn(
                name: "Birthday",
                table: "t_1000Staff",
                newName: "f_staff_Bitrthday");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "t_1000Staff",
                newName: "f_staff_ID");

            migrationBuilder.AlterColumn<int>(
                name: "StaffId",
                table: "t_1001StaffCertificates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_Account",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_BankAccount",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_BankName",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_BloodType",
                table: "t_1000Staff",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_BusinessPhone",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_ChineseName",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_ContactAddress",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_ContactPhone",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC1Address",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC1Cellphone",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC1Name",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC1Relationship",
                table: "t_1000Staff",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC2Address",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC2Cellphone",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC2Name",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EC2Relationship",
                table: "t_1000Staff",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_Email",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_EnglishName",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "f_staff_ExtensionNumber",
                table: "t_1000Staff",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_Gender",
                table: "t_1000Staff",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_HighestEducation",
                table: "t_1000Staff",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_IDCard",
                table: "t_1000Staff",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_staff_LineID",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_OfficialMail",
                table: "t_1000Staff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_SubBankName",
                table: "t_1000Staff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
