using Microsoft.EntityFrameworkCore;
using server.Services;
using ToDoEasyApp.Data;
using ToDoEasyApp.Models;

namespace ToDoEasyApp.Services
{

    public class TodoItemService : ITodoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoItemService> _logger;

        public TodoItemService(ApplicationDbContext context, ILogger<TodoItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> TodoItemExistsAsync(int id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItemsByUserAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoItemDto>> GetAllTodoItemsAsync()
        {
            var todoItems = await _context.TodoItems.ToListAsync();
            return todoItems.Select(MapToDto);
        }


        public async Task<TodoItem> CreateTodoItemAsync(TodoItemDto todoItemDto, string userId)
        {
            TodoItem todoItem = new TodoItem
            {
                Title = todoItemDto.Title,
                IsCompleted = todoItemDto.IsCompleted,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task<TodoItemDto?> UpdateTodoItemAsync(TodoItemDto updatedTodoItemDto)
        {

            var todoItem = await _context.TodoItems.FindAsync(updatedTodoItemDto.Id);
            if (todoItem == null)
            {
                _logger.LogWarning($"There is no todoItem with id {updatedTodoItemDto.Id}");
                throw new KeyNotFoundException($"Todo item with id {updatedTodoItemDto.Id} was not found");
            }

            // updating 
            todoItem.Title = updatedTodoItemDto.Title;
            todoItem.IsCompleted = updatedTodoItemDto.IsCompleted;

            await _context.SaveChangesAsync();

            return MapToDto(todoItem);
        }

        public async Task<bool> DeleteTodoItemAsync(int todoItemId)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemId);
            if (todoItem == null)
            {
                _logger.LogWarning($"There is no todoItem with id {todoItemId}");
                throw new KeyNotFoundException($"TodoItem with id {todoItemId} not found");
            }

            // hard DELETE
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            _logger.LogWarning($"Deleted todoItem with id {todoItem}");
            return true;
        }

        public async Task<TodoItemDto?> GetTodoItemByPropertiesAsync(string? title, bool? isCompleted)
        {
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(x =>
                (!string.IsNullOrWhiteSpace(title) && x.Title.ToLower() == title.ToLower()) &&
                isCompleted.HasValue && x.IsCompleted == isCompleted.Value);

            return todoItem == null ? null : MapToDto(todoItem);
        }

        public async Task<TodoItemDto?> GetTodoItemByIdAsync(int todoItemId)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemId);
            if (todoItem == null)
            {
                _logger.LogWarning($"There is no todoItem with id {todoItemId}");
                throw new KeyNotFoundException($"Todo item with id {todoItemId} was not found");
            }
            return MapToDto(todoItem);
        }

        // маппинг между TodoItem и TodoItemDto
        public TodoItemDto MapToDto(TodoItem todoItem)
        {
            return new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                IsCompleted = todoItem.IsCompleted
            };
        }

        public async Task DeleteAllTodoItemsAsync()
        {
            _context.TodoItems.RemoveRange(_context.TodoItems);
            await _context.SaveChangesAsync();
        }

    }
}
