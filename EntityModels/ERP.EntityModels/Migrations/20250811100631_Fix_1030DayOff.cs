using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_1030DayOff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_StaffId",
                table: "t_1030Dayoff");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                table: "t_1030Dayoff",
                newName: "LeaveTaker");

            migrationBuilder.RenameIndex(
                name: "IX_t_1030Dayoff_StaffId",
                table: "t_1030Dayoff",
                newName: "IX_t_1030Dayoff_LeaveTaker");

            migrationBuilder.AlterColumn<int>(
                name: "Proxy",
                table: "t_1030Dayoff",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Applicant",
                table: "t_1030Dayoff",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_1030Dayoff_Applicant",
                table: "t_1030Dayoff",
                column: "Applicant");

            migrationBuilder.CreateIndex(
                name: "IX_t_1030Dayoff_Proxy",
                table: "t_1030Dayoff",
                column: "Proxy");

            migrationBuilder.AddForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_Applicant",
                table: "t_1030Dayoff",
                column: "Applicant",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_LeaveTaker",
                table: "t_1030Dayoff",
                column: "LeaveTaker",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_Proxy",
                table: "t_1030Dayoff",
                column: "Proxy",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_Applicant",
                table: "t_1030Dayoff");

            migrationBuilder.DropForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_LeaveTaker",
                table: "t_1030Dayoff");

            migrationBuilder.DropForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_Proxy",
                table: "t_1030Dayoff");

            migrationBuilder.DropIndex(
                name: "IX_t_1030Dayoff_Applicant",
                table: "t_1030Dayoff");

            migrationBuilder.DropIndex(
                name: "IX_t_1030Dayoff_Proxy",
                table: "t_1030Dayoff");

            migrationBuilder.RenameColumn(
                name: "LeaveTaker",
                table: "t_1030Dayoff",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_t_1030Dayoff_LeaveTaker",
                table: "t_1030Dayoff",
                newName: "IX_t_1030Dayoff_StaffId");

            migrationBuilder.AlterColumn<string>(
                name: "Proxy",
                table: "t_1030Dayoff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Applicant",
                table: "t_1030Dayoff",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_t_1030Dayoff_t_1000Staff_StaffId",
                table: "t_1030Dayoff",
                column: "StaffId",
                principalTable: "t_1000Staff",
                principalColumn: "StaffId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
