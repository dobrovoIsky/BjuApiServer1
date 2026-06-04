using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BjuApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddRankingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStreak",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastGoalReachedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStreak",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastGoalReachedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "Users");
        }
    }
}
