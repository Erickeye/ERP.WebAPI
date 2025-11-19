using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_SomeColume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "f_ActionInfo_Account",
                table: "t_1710ActionInfo",
                newName: "Account");

            migrationBuilder.RenameColumn(
                name: "f_ActionInfo_UID",
                table: "t_1710ActionInfo",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "LoginId",
                table: "t_1700LoginLog",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailVehicle",
                table: "t_1201PettyCashDetail",
                newName: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailTotal",
                table: "t_1201PettyCashDetail",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailTax",
                table: "t_1201PettyCashDetail",
                newName: "Tax");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailSupplier",
                table: "t_1201PettyCashDetail",
                newName: "Supplier");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailSort",
                table: "t_1201PettyCashDetail",
                newName: "Sort");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailPurchaseId",
                table: "t_1201PettyCashDetail",
                newName: "PurchaseId");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailProject",
                table: "t_1201PettyCashDetail",
                newName: "Project");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailName",
                table: "t_1201PettyCashDetail",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailInvoiceNumber",
                table: "t_1201PettyCashDetail",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailInvoiceDate",
                table: "t_1201PettyCashDetail",
                newName: "InvoiceDate");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailContent",
                table: "t_1201PettyCashDetail",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailAmount",
                table: "t_1201PettyCashDetail",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "PettyCashDetailId",
                table: "t_1201PettyCashDetail",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PettyCashId",
                table: "t_1200PettyCash",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "t_1080Company",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WorkOverId",
                table: "t_1050WorkOver",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "t_1040Document",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DayOffId",
                table: "t_1030Dayoff",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Account",
                table: "t_1710ActionInfo",
                newName: "f_ActionInfo_Account");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1710ActionInfo",
                newName: "f_ActionInfo_UID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1700LoginLog",
                newName: "LoginId");

            migrationBuilder.RenameColumn(
                name: "Vehicle",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailVehicle");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailTotal");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailTax");

            migrationBuilder.RenameColumn(
                name: "Supplier",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailSupplier");

            migrationBuilder.RenameColumn(
                name: "Sort",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailSort");

            migrationBuilder.RenameColumn(
                name: "PurchaseId",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailPurchaseId");

            migrationBuilder.RenameColumn(
                name: "Project",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailProject");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailName");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailInvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailInvoiceDate");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailContent");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailAmount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1201PettyCashDetail",
                newName: "PettyCashDetailId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1200PettyCash",
                newName: "PettyCashId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1080Company",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1050WorkOver",
                newName: "WorkOverId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1040Document",
                newName: "DocumentId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_1030Dayoff",
                newName: "DayOffId");
        }
    }
}
