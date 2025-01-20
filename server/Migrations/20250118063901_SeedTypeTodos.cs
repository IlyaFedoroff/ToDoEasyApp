using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoEasyApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedTypeTodos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TypeTodos",
                columns: new[] { "Name" },
                values: new object[,]
                {
                    { "Работа" },
                    { "Личное" },
                    { "Учеба" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TypeTodos",
                keyColumn: "Name",
                keyValue: new object[] { "Работа", "Личное", "Учеба" });
        }
    }
}
