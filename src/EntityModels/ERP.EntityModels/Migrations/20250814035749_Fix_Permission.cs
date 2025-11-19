using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                columns: new[] { "RoleId", "PageId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Permission",
                table: "Permission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permission",
                table: "Permission",
                column: "RoleId");
        }
    }
}
