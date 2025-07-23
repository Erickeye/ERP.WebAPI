using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_level",
                columns: table => new
                {
                    f_permissionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_permissionLevel = table.Column<int>(type: "int", nullable: false),
                    f_levelAmount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_level", x => x.f_permissionID);
                });

            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    f_roleId = table.Column<int>(type: "int", nullable: false),
                    f_pageId = table.Column<int>(type: "int", nullable: false),
                    f_type = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_permission", x => new { x.f_roleId, x.f_pageId });
                });

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    f_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_roleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_sessionTime = table.Column<int>(type: "int", nullable: true),
                    f_permissionLevel = table.Column<int>(type: "int", nullable: true),
                    f_approvalLevel = table.Column<int>(type: "int", nullable: true),
                    f_quotationLevel = table.Column<int>(type: "int", nullable: true),
                    f_procurementLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.f_id);
                });

            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    f_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    f_createDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    f_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_pwd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    f_isLock = table.Column<bool>(type: "bit", nullable: false),
                    f_sessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    f_role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.f_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_level");

            migrationBuilder.DropTable(
                name: "t_permission");

            migrationBuilder.DropTable(
                name: "t_role");

            migrationBuilder.DropTable(
                name: "t_user");
        }
    }
}
