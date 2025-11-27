using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BjuApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutPlans : Migration
    {
       

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealPlans_Users_UserId",
                table: "MealPlans");

            migrationBuilder.DropIndex(
                name: "IX_MealPlans_UserId",
                table: "MealPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_UserId",
                table: "MealPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealPlans_Users_UserId",
                table: "MealPlans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
