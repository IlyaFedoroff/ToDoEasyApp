using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ToDoEasyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedTypeTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "TodoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeTodoId",
                table: "TodoItems",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TypeTodo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeTodo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TypeId",
                table: "TodoItems",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TypeTodoId",
                table: "TodoItems",
                column: "TypeTodoId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeId",
                table: "TodoItems",
                column: "TypeId",
                principalTable: "TypeTodo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeTodoId",
                table: "TodoItems",
                column: "TypeTodoId",
                principalTable: "TypeTodo",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeTodoId",
                table: "TodoItems");

            migrationBuilder.DropTable(
                name: "TypeTodo");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TypeId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TypeTodoId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "TypeTodoId",
                table: "TodoItems");
        }
    }
}
