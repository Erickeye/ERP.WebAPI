using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_2000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_t_2010custemploy",
                table: "t_2010custemploy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_2000customer",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Account",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Department",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Email",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_EmotionState",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_ExtNum",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Job",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_MobilePhone",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Momo",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Name",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Post",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custemploy_Remark",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_customer_AttribName",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_customer_Name",
                table: "t_2010custemploy");

            migrationBuilder.DropColumn(
                name: "f_custome_InvoiceForm",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_AttribName",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_BankName",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_CheckingAccount",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_ContactPhone",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_DeliveryAddress",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_FaxPhone",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_Name",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_Owner",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_RegisteredAddress",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_RemittanceAccount",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_TaxInvoiceAddress",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_customer_UID",
                table: "t_2000customer");

            migrationBuilder.DropColumn(
                name: "f_staff_ChineseName",
                table: "t_2000customer");

            migrationBuilder.RenameTable(
                name: "t_2010custemploy",
                newName: "t_2010Custemploy");

            migrationBuilder.RenameTable(
                name: "t_2000customer",
                newName: "t_2000Customer");

            migrationBuilder.RenameColumn(
                name: "f_customer_ID",
                table: "t_2010Custemploy",
                newName: "MarriageStatus");

            migrationBuilder.RenameColumn(
                name: "f_custemploy_ID",
                table: "t_2010Custemploy",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "f_customer_TaxInvoiceNumber",
                table: "t_2000Customer",
                newName: "TaxInvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "f_customer_PayDays",
                table: "t_2000Customer",
                newName: "PayDays");

            migrationBuilder.RenameColumn(
                name: "f_customer_LastDeliveryDate",
                table: "t_2000Customer",
                newName: "LastDeliveryDate");

            migrationBuilder.RenameColumn(
                name: "f_customer_CreditLine",
                table: "t_2000Customer",
                newName: "CreditLine");

            migrationBuilder.RenameColumn(
                name: "f_customer_CreditBalance",
                table: "t_2000Customer",
                newName: "CreditBalance");

            migrationBuilder.RenameColumn(
                name: "f_customer_Advance",
                table: "t_2000Customer",
                newName: "Advance");

            migrationBuilder.RenameColumn(
                name: "f_customer_ID",
                table: "t_2000Customer",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "t_2010Custemploy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "t_2010Custemploy",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtNum",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Job",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobStatus",
                table: "t_2010Custemploy",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Momo",
                table: "t_2010Custemploy",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "t_2010Custemploy",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttribName",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CheckingAccount",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "t_2000Customer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxPhone",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceForm",
                table: "t_2000Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "t_2000Customer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegisteredAddress",
                table: "t_2000Customer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemittanceAccount",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffChineseName",
                table: "t_2000Customer",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaxInvoiceAddress",
                table: "t_2000Customer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_2010Custemploy",
                table: "t_2010Custemploy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_2000Customer",
                table: "t_2000Customer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_t_2010Custemploy_CustomerId",
                table: "t_2010Custemploy",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_2010Custemploy_t_2000Customer_CustomerId",
                table: "t_2010Custemploy",
                column: "CustomerId",
                principalTable: "t_2000Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_2010Custemploy_t_2000Customer_CustomerId",
                table: "t_2010Custemploy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_2010Custemploy",
                table: "t_2010Custemploy");

            migrationBuilder.DropIndex(
                name: "IX_t_2010Custemploy_CustomerId",
                table: "t_2010Custemploy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_2000Customer",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "Account",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "ExtNum",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "Job",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "JobStatus",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "Momo",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "t_2010Custemploy");

            migrationBuilder.DropColumn(
                name: "AttribName",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "CheckingAccount",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "FaxPhone",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "InvoiceForm",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "RegisteredAddress",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "RemittanceAccount",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "StaffChineseName",
                table: "t_2000Customer");

            migrationBuilder.DropColumn(
                name: "TaxInvoiceAddress",
                table: "t_2000Customer");

            migrationBuilder.RenameTable(
                name: "t_2010Custemploy",
                newName: "t_2010custemploy");

            migrationBuilder.RenameTable(
                name: "t_2000Customer",
                newName: "t_2000customer");

            migrationBuilder.RenameColumn(
                name: "MarriageStatus",
                table: "t_2010custemploy",
                newName: "f_customer_ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_2010custemploy",
                newName: "f_custemploy_ID");

            migrationBuilder.RenameColumn(
                name: "TaxInvoiceNumber",
                table: "t_2000customer",
                newName: "f_customer_TaxInvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "PayDays",
                table: "t_2000customer",
                newName: "f_customer_PayDays");

            migrationBuilder.RenameColumn(
                name: "LastDeliveryDate",
                table: "t_2000customer",
                newName: "f_customer_LastDeliveryDate");

            migrationBuilder.RenameColumn(
                name: "CreditLine",
                table: "t_2000customer",
                newName: "f_customer_CreditLine");

            migrationBuilder.RenameColumn(
                name: "CreditBalance",
                table: "t_2000customer",
                newName: "f_customer_CreditBalance");

            migrationBuilder.RenameColumn(
                name: "Advance",
                table: "t_2000customer",
                newName: "f_customer_Advance");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_2000customer",
                newName: "f_customer_ID");

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Account",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Department",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Email",
                table: "t_2010custemploy",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_EmotionState",
                table: "t_2010custemploy",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_ExtNum",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Job",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_MobilePhone",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Momo",
                table: "t_2010custemploy",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Name",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Post",
                table: "t_2010custemploy",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_custemploy_Remark",
                table: "t_2010custemploy",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_AttribName",
                table: "t_2010custemploy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_customer_Name",
                table: "t_2010custemploy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_custome_InvoiceForm",
                table: "t_2000customer",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_AttribName",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_customer_BankName",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_CheckingAccount",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_ContactPhone",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_DeliveryAddress",
                table: "t_2000customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_FaxPhone",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_Name",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_customer_Owner",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_customer_RegisteredAddress",
                table: "t_2000customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "f_customer_RemittanceAccount",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_TaxInvoiceAddress",
                table: "t_2000customer",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_customer_UID",
                table: "t_2000customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "f_staff_ChineseName",
                table: "t_2000customer",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_2010custemploy",
                table: "t_2010custemploy",
                column: "f_custemploy_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_2000customer",
                table: "t_2000customer",
                column: "f_customer_ID");
        }
    }
}
