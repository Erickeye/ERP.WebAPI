using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Add_ERP_2000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_2000customer");

            migrationBuilder.DropTable(
                name: "t_2010custemploy");
        }
    }
}
