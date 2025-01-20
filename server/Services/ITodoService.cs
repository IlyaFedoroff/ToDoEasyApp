using server.Models;
using ToDoEasyApp.Models;

namespace server.Services
{
    public interface ITodoService
    {
        Task<TodoItemDto> AddTodoItemAsync(TodoItemDto todoItemDto, string userId);
        Task<IEnumerable<TodoItem>> GetTodoItemsByUserAsync(string userId);

        Task<IEnumerable<TypeTodoDto>> GetTypesAsync();

        Task<IEnumerable<TodoItemForSearchDto>> GetAllTodoItemsAsync();

        Task<List<TodoItemForSearchDto>> SearchTodosAsync(DateTime? createdAt, int? typeId, string? authorName);

        Task<List<TodoItemForSearchDto>> SearchTodosDapperAsync(DateTime? createdAt, int? typeId, string? authorName);


        Task<TodoItemDto?> UpdateTodoItemAsync(TodoItemDto updatedTodSoItemDto);

        Task<bool> DeleteTodoItemAsync(int todoItemId);

        Task<TodoItemDto?> GetTodoItemByPropertiesAsync(string? title, bool? isCompleted);

        TodoItemDto MapToDto(TodoItem todoItem);

        Task<TodoItemDto?> GetTodoItemByIdAsync(int todoItemId);


    }
}
