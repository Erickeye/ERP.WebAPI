using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_1200_1710 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_t_1200PettyCash",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_Controller",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_ControllerID",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_Date",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_Function",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_IP",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_ActionInfo_describe",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "f_login_CrateDate",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "f_login_IP",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "f_login_Type",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "f_PettyCashDetail_Content",
                table: "t_1201PettyCashDetail");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_ID",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Accountant",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Authorizator",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Company",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Filler",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Handler",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Manager",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Payee",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Reason",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "f_PettyCash_Supervisor",
                table: "t_1200PettyCash");

            migrationBuilder.RenameColumn(
                name: "f_staff_Account",
                table: "t_1700LoginLog",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "f_login_UID",
                table: "t_1700LoginLog",
                newName: "LoginId");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_ID",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailName");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Vehicle",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailVehicle");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Total",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailTotal");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Tax",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailTax");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Supplier",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailSupplier");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Sort",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailSort");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_PurchaseID",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailPurchaseId");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Project",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailProject");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Name",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailInvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_InvoiceNumber",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailContent");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_InvoiceDate",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailInvoiceDate");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_Amount",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailAmount");

            migrationBuilder.RenameColumn(
                name: "f_PettyCashDetail_ID",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailId");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_TotalAmount",
                table: "t_1200PettyCash",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_PaymentDate",
                table: "t_1200PettyCash",
                newName: "PaymentDate");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_Date",
                table: "t_1200PettyCash",
                newName: "RequestDate");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_Approval",
                table: "t_1200PettyCash",
                newName: "Approval");

            migrationBuilder.RenameColumn(
                name: "f_PettyCash_Accounting",
                table: "t_1200PettyCash",
                newName: "Accounting");

            migrationBuilder.AlterColumn<string>(
                name: "f_ActionInfo_Account",
                table: "t_1710ActionInfo",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "t_1710ActionInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CrateDate",
                table: "t_1710ActionInfo",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "t_1710ActionInfo",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeyId",
                table: "t_1710ActionInfo",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "t_1710ActionInfo",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Memo",
                table: "t_1710ActionInfo",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "t_1710ActionInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CrateDate",
                table: "t_1700LoginLog",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "t_1700LoginLog",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "t_1700LoginLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "t_1700LoginLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PettyCashId",
                table: "t_1201PettyCashDetail",
                type: "nvarchar(32)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PettyCashId",
                table: "t_1200PettyCash",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Authorizator",
                table: "t_1200PettyCash",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "t_1200PettyCash",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Filler",
                table: "t_1200PettyCash",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Payee",
                table: "t_1200PettyCash",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "t_1200PettyCash",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "t_1200PettyCash",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_1200PettyCash",
                table: "t_1200PettyCash",
                column: "PettyCashId");

            migrationBuilder.CreateIndex(
                name: "IX_t_1201PettyCashDetail_PettyCashId",
                table: "t_1201PettyCashDetail",
                column: "PettyCashId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_1201PettyCashDetail_t_1200PettyCash_PettyCashId",
                table: "t_1201PettyCashDetail",
                column: "PettyCashId",
                principalTable: "t_1200PettyCash",
                principalColumn: "PettyCashId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_1201PettyCashDetail_t_1200PettyCash_PettyCashId",
                table: "t_1201PettyCashDetail");

            migrationBuilder.DropIndex(
                name: "IX_t_1201PettyCashDetail_PettyCashId",
                table: "t_1201PettyCashDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_1200PettyCash",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "CrateDate",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "KeyId",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "Memo",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "t_1710ActionInfo");

            migrationBuilder.DropColumn(
                name: "CrateDate",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "t_1700LoginLog");

            migrationBuilder.DropColumn(
                name: "PettyCashId",
                table: "t_1201PettyCashDetail");

            migrationBuilder.DropColumn(
                name: "PettyCashId",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Authorizator",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Filler",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Payee",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "t_1200PettyCash");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "t_1200PettyCash");

            migrationBuilder.RenameColumn(
                name: "Account",
                table: "t_1700LoginLog",
                newName: "f_staff_Account");

            migrationBuilder.RenameColumn(
                name: "LoginId",
                table: "t_1700LoginLog",
                newName: "f_login_UID");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailVehicle",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Vehicle");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailTotal",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Total");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailTax",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Tax");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailSupplier",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Supplier");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailSort",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Sort");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailPurchaseId",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_PurchaseID");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailProject",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Project");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailName",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCash_ID");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailInvoiceNumber",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Name");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailInvoiceDate",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_InvoiceDate");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailContent",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailAmount",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_Amount");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailId",
                table: "t_1201PettyCashDetail",
                newName: "f_PettyCashDetail_ID");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "t_1200PettyCash",
                newName: "f_PettyCash_TotalAmount");

            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "t_1200PettyCash",
                newName: "f_PettyCash_Date");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "t_1200PettyCash",
                newName: "f_PettyCash_PaymentDate");

            migrationBuilder.RenameColumn(
                name: "Approval",
                table: "t_1200PettyCash",
                newName: "f_PettyCash_Approval");

            migrationBuilder.RenameColumn(
                name: "Accounting",
                table: "t_1200PettyCash",
                newName: "f_PettyCash_Accounting");

            migrationBuilder.AlterColumn<string>(
                name: "f_ActionInfo_Account",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "f_ActionInfo_Controller",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_ActionInfo_ControllerID",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "f_ActionInfo_Date",
                table: "t_1710ActionInfo",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_ActionInfo_Function",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_ActionInfo_IP",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_ActionInfo_describe",
                table: "t_1710ActionInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "f_login_CrateDate",
                table: "t_1700LoginLog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_login_IP",
                table: "t_1700LoginLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "f_login_Type",
                table: "t_1700LoginLog",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCashDetail_Content",
                table: "t_1201PettyCashDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_ID",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Accountant",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Authorizator",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Company",
                table: "t_1200PettyCash",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Filler",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Handler",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Manager",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Payee",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Reason",
                table: "t_1200PettyCash",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_PettyCash_Supervisor",
                table: "t_1200PettyCash",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_1200PettyCash",
                table: "t_1200PettyCash",
                column: "f_PettyCash_ID");
        }
    }
}
