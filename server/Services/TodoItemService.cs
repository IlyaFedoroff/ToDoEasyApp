using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ToDoEasyApp.Data;
using ToDoEasyApp.Models;

namespace ToDoEasyApp.Services
{
    public class TodoItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoItemService> _logger;

        public TodoItemService(ApplicationDbContext context, ILogger<TodoItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> TodoItemExists(int id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

        public async Task<TodoItemDto?> GetTodoItem(int id)
        {
            _logger.LogInformation($"Getting TodoItem with id {id}");
            var todoItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
            if (todoItem == null)
                _logger.LogWarning($"TodoItem with id {id} is not found");
            return todoItem == null ? null : MapToDto(todoItem);
        }

        public async Task<IEnumerable<TodoItemDto>> GetAllTodoItems()
        {
            var todoItems = await _context.TodoItems.ToListAsync();
            return todoItems.Select(MapToDto);
        }
        // how adding
        public async Task<TodoItemDto> AddTodoItem(TodoItemDto todoItemDto)
        {
            // Создаём новый объект TodoItem
            TodoItem todoItem = new TodoItem
            {
                Title = todoItemDto.Title,
                IsCompleted = todoItemDto.IsCompleted,
                CreatedAt = DateTime.UtcNow // Дата создания, если она нужна
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return MapToDto(todoItem);
        }

        public async Task<TodoItemDto?> UpdateTodoItem(TodoItemDto updatedTodoItemDto, int id)
        {

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                _logger.LogWarning($"There is no todoItem with id {id}");
                return null;    //   если елемент не найден
            }

            // updating 
            todoItem.Title = updatedTodoItemDto.Title;
            todoItem.IsCompleted = updatedTodoItemDto.IsCompleted;

            await _context.SaveChangesAsync();

            return MapToDto(todoItem);
        }

        public async Task<bool> DeleteTodoItem(int todoItemid)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemid);
            if (todoItem == null)
            {
                _logger.LogWarning($"There is no todoItem with id {todoItemid}");
                return false;
            }

            // hard DELETE
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            _logger.LogWarning($"Deleted todoItem with id {todoItem}");
            return true;
        }

        public async Task<TodoItemDto?> GetTodoItemByProperties(string? title, bool? isCompleted)
        {
            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(x =>
                (!string.IsNullOrWhiteSpace(title) && x.Title.ToLower() == title.ToLower()) &&
                isCompleted.HasValue && x.IsCompleted == isCompleted.Value);

            return todoItem == null ? null : MapToDto(todoItem);
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

        public async Task DeleteAllTodoItems()
        {
            _context.TodoItems.RemoveRange(_context.TodoItems);
            await _context.SaveChangesAsync();
        }

    }


}
