using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.EntityModels.Migrations
{
    /// <inheritdoc />
    public partial class Add_Users_2col : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "f_refreshToken",
                table: "t_user",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "f_refreshTokenExpiryTime",
                table: "t_user",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "f_refreshToken",
                table: "t_user");

            migrationBuilder.DropColumn(
                name: "f_refreshTokenExpiryTime",
                table: "t_user");
        }
    }
}
