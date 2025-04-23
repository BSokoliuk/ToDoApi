using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "todo_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    percent_complete = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    expiry_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_todo_items", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "todo_items",
                columns: new[] { "id", "created_at", "description", "expiry_date_time", "last_updated_at", "percent_complete", "title" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "This is a sample todo item.", new DateTime(2023, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, "Sample Todo Item" },
                    { 2, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "This is another todo item.", new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, "Another Todo Item" }
                });

            migrationBuilder.InsertData(
                table: "todo_items",
                columns: new[] { "id", "created_at", "description", "expiry_date_time", "is_completed", "last_updated_at", "percent_complete", "title" },
                values: new object[] { 3, new DateTime(2022, 12, 22, 0, 0, 0, 0, DateTimeKind.Utc), "This todo item is completed.", new DateTime(2022, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 0, "Completed Todo Item" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "todo_items");
        }
    }
}
