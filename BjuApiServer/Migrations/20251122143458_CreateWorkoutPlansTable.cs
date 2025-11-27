using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BjuApiServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorkoutPlansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkoutPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Goal = table.Column<string>(type: "TEXT", nullable: false),
                    Intensity = table.Column<string>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanText = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutPlans", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutPlans");
        }
    }
}
