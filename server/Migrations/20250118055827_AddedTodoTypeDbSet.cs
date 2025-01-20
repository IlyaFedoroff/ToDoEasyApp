using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoEasyApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedTodoTypeDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodo_TypeTodoId",
                table: "TodoItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeTodo",
                table: "TypeTodo");

            migrationBuilder.RenameTable(
                name: "TypeTodo",
                newName: "TypeTodos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeTodos",
                table: "TypeTodos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TypeTodos_TypeId",
                table: "TodoItems",
                column: "TypeId",
                principalTable: "TypeTodos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TypeTodos_TypeTodoId",
                table: "TodoItems",
                column: "TypeTodoId",
                principalTable: "TypeTodos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodos_TypeId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TypeTodos_TypeTodoId",
                table: "TodoItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TypeTodos",
                table: "TypeTodos");

            migrationBuilder.RenameTable(
                name: "TypeTodos",
                newName: "TypeTodo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TypeTodo",
                table: "TypeTodo",
                column: "Id");

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
    }
}
