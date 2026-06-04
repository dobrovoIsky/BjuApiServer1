using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BjuApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "Users",
                newName: "MonthlyPoints");

            migrationBuilder.AddColumn<int>(
                name: "Balance",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastMonthlyReset",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastMonthlyReset",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MonthlyPoints",
                table: "Users",
                newName: "Points");
        }
    }
}
