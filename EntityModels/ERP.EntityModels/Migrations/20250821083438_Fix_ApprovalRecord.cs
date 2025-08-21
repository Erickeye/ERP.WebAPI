using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ApprovalRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TableType",
                table: "ApprovalRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableType",
                table: "ApprovalRecord");
        }
    }
}
