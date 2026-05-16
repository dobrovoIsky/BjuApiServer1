using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BjuApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavoriteToMealPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "MealPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "MealPlans");
        }
    }
}
