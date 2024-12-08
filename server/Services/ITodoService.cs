using ToDoEasyApp.Models;

namespace server.Services
{
    public interface ITodoService
    {
        Task<TodoItem> CreateTodoItemAsync(TodoItemDto todoItemDto, string userId);
        Task<IEnumerable<TodoItem>> GetTodoItemsByUserAsync(string userId);

        Task<IEnumerable<TodoItemDto>> GetAllTodoItemsAsync();

        Task<TodoItemDto?> UpdateTodoItemAsync(TodoItemDto updatedTodoItemDto);

        Task<bool> DeleteTodoItemAsync(int todoItemId);

        Task<TodoItemDto?> GetTodoItemByPropertiesAsync(string? title, bool? isCompleted);

        TodoItemDto MapToDto(TodoItem todoItem);

        Task<TodoItemDto?> GetTodoItemByIdAsync(int todoItemId);


    }
}
