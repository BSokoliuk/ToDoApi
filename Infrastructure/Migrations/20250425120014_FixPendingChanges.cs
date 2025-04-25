using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "todo_items",
                keyColumn: "id",
                keyValue: 2,
                column: "percent_complete",
                value: 50);

            migrationBuilder.UpdateData(
                table: "todo_items",
                keyColumn: "id",
                keyValue: 3,
                column: "percent_complete",
                value: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "todo_items",
                keyColumn: "id",
                keyValue: 2,
                column: "percent_complete",
                value: 0);

            migrationBuilder.UpdateData(
                table: "todo_items",
                keyColumn: "id",
                keyValue: 3,
                column: "percent_complete",
                value: 0);
        }
    }
}
