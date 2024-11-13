using Microsoft.EntityFrameworkCore;
using ToDoEasyApp.Data;
using ToDoEasyApp.Models;

namespace ToDoEasyApp.Services
{
    public class TodoItemService
    {
        private readonly ApplicationDbContext _context;

        public TodoItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> TodoItemExists(int id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

        public async Task<TodoItem?> GetTodoItem(int id)
        {
            return await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        public async Task<TodoItem> AddTodoItem(TodoItem todoItem)
        {
            int maxId = _context.TodoItems.Max(x => x.Id);
            todoItem.Id = maxId + 1;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task<TodoItem?> UpdateTodoItem(TodoItem updatedTodoItem, int id)
        {
            
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return null;    //   если елемент не найден
            }

            // updating 
            todoItem.Title = updatedTodoItem.Title;
            todoItem.IsCompleted = updatedTodoItem.IsCompleted;

            await _context.SaveChangesAsync();

            return todoItem;
        }

        public async Task<bool> DeleteTodoItem(int todoItemid)
        {
            var todoItem = await GetTodoItem(todoItemid);

            if (todoItem != null)
            {
                // hard DELETE
                _context.TodoItems.Remove(todoItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<TodoItem?> GetTodoItemByProperties(string? title, bool? isCompleted)
        {
            return await _context.TodoItems.FirstOrDefaultAsync(x =>
            !string.IsNullOrWhiteSpace(title) &&
            !string.IsNullOrWhiteSpace(x.Title) &&
            x.Title.ToLower() == title.ToLower() &&
            isCompleted.HasValue &&
            x.IsCompleted.HasValue &&
            isCompleted.Value == x.IsCompleted.Value);
        }


    }


}
