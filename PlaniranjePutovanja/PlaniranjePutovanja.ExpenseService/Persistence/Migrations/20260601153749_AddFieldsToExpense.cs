using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaniranjePutovanja.ExpenseService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemGenerated",
                table: "Expenses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "IsSystemGenerated",
                table: "Expenses");
        }
    }
}
