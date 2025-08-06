using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_1030_1040 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "f_Document_Attachment",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Authorizator",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Company",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_ContactPerson",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Contract",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Date",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Level",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Original",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Recipient",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Remark2",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Document_Subject",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Applicant",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Authorizator",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Department",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_LeaveType",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Name",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Position",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "f_Dayoff_Reason",
                table: "t_1030Dayoff");

            migrationBuilder.RenameColumn(
                name: "f_Document_Remark1",
                table: "t_1040Document",
                newName: "Remark1");

            migrationBuilder.RenameColumn(
                name: "f_Document_File",
                table: "t_1040Document",
                newName: "File");

            migrationBuilder.RenameColumn(
                name: "f_Document_Approval",
                table: "t_1040Document",
                newName: "Approval");

            migrationBuilder.RenameColumn(
                name: "f_Document_ID",
                table: "t_1040Document",
                newName: "DocumentId");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_ProxySign",
                table: "t_1030Dayoff",
                newName: "Authorizer");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_EndDate",
                table: "t_1030Dayoff",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_Date",
                table: "t_1030Dayoff",
                newName: "ApplicationDate");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_BeginDate",
                table: "t_1030Dayoff",
                newName: "BeginDate");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_Approval",
                table: "t_1030Dayoff",
                newName: "ApprovalStatus");

            migrationBuilder.RenameColumn(
                name: "f_Dayoff_ID",
                table: "t_1030Dayoff",
                newName: "DayOffId");

            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "t_1040Document",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Authorizator",
                table: "t_1040Document",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "t_1040Document",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "t_1040Document",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contract",
                table: "t_1040Document",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentDate",
                table: "t_1040Document",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "t_1040Document",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Original",
                table: "t_1040Document",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "t_1040Document",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remark2",
                table: "t_1040Document",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "t_1040Document",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Applicant",
                table: "t_1030Dayoff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "t_1030Dayoff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeaveType",
                table: "t_1030Dayoff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Proxy",
                table: "t_1030Dayoff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProxySignature",
                table: "t_1030Dayoff",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "t_1030Dayoff",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "t_1030Dayoff",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_t_1039DayoffProxy_DayoffId",
                table: "t_1039DayoffProxy",
                column: "DayoffId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1030Dayoff_StaffId",
                table: "t_1030Dayoff",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_StaffId",
                table: "t_1030Dayoff",
                column: "StaffId",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_1039DayoffProxy_t_1030Dayoff_DayoffId",
                table: "t_1039DayoffProxy",
                column: "DayoffId",
                principalTable: "t_1030Dayoff",
                principalColumn: "DayOffId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_StaffId",
                table: "t_1030Dayoff");

            migrationBuilder.DropForeignKey(
                name: "FK_t_1039DayoffProxy_t_1030Dayoff_DayoffId",
                table: "t_1039DayoffProxy");

            migrationBuilder.DropIndex(
                name: "IX_t_1039DayoffProxy_DayoffId",
                table: "t_1039DayoffProxy");

            migrationBuilder.DropIndex(
                name: "IX_t_1030Dayoff_StaffId",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Authorizator",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Contract",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "DocumentDate",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Original",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Remark2",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "t_1040Document");

            migrationBuilder.DropColumn(
                name: "Applicant",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "LeaveType",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "Proxy",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "ProxySignature",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "t_1030Dayoff");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "t_1030Dayoff");

            migrationBuilder.RenameColumn(
                name: "Remark1",
                table: "t_1040Document",
                newName: "f_Document_Remark1");

            migrationBuilder.RenameColumn(
                name: "File",
                table: "t_1040Document",
                newName: "f_Document_File");

            migrationBuilder.RenameColumn(
                name: "Approval",
                table: "t_1040Document",
                newName: "f_Document_Approval");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "t_1040Document",
                newName: "f_Document_ID");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_EndDate");

            migrationBuilder.RenameColumn(
                name: "BeginDate",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_BeginDate");

            migrationBuilder.RenameColumn(
                name: "Authorizer",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_ProxySign");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_Approval");

            migrationBuilder.RenameColumn(
                name: "ApplicationDate",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_Date");

            migrationBuilder.RenameColumn(
                name: "DayOffId",
                table: "t_1030Dayoff",
                newName: "f_Dayoff_ID");

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Attachment",
                table: "t_1040Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Authorizator",
                table: "t_1040Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Company",
                table: "t_1040Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_ContactPerson",
                table: "t_1040Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Contract",
                table: "t_1040Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "f_Document_Date",
                table: "t_1040Document",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Level",
                table: "t_1040Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Original",
                table: "t_1040Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Recipient",
                table: "t_1040Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Remark2",
                table: "t_1040Document",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Document_Subject",
                table: "t_1040Document",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Applicant",
                table: "t_1030Dayoff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Authorizator",
                table: "t_1030Dayoff",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Department",
                table: "t_1030Dayoff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_LeaveType",
                table: "t_1030Dayoff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Name",
                table: "t_1030Dayoff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Position",
                table: "t_1030Dayoff",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_Dayoff_Reason",
                table: "t_1030Dayoff",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
