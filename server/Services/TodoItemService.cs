using Microsoft.EntityFrameworkCore;
using server.Models;
using server.Services;
using ToDoEasyApp.Data;
using ToDoEasyApp.Models;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace ToDoEasyApp.Services
{

    public class TodoItemService : ITodoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoItemService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "";

        public TodoItemService(ApplicationDbContext context,
            ILogger<TodoItemService> logger,
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
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

        public async Task<IEnumerable<TodoItemForSearchDto>> GetAllTodoItemsAsync()
        {
            var todoItems = await _context.TodoItems
                .Include(t => t.Type)
                .Include(t => t.User)
                .Select(t => new TodoItemForSearchDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    TypeName = t.Type != null ? t.Type.Name : "Неизвестно",
                    AuthorName = t.User != null ? $"{t.User.FirstName} {t.User.LastName}" : "Неизвестный автор"
                })
                .ToListAsync();

            return todoItems;
        }

        public async Task<IEnumerable<TypeTodoDto>> GetTypesAsync()
        {
            _logger.LogInformation("Обращение к сервису для получения типов");
            return await _context.TypeTodos
                .Select(t => new TypeTodoDto
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }

        public async Task<TodoItemDto> AddTodoItemAsync(TodoItemDto todoItemDto, string userId)
        {
            var todoItemToAdd = new TodoItem
            {
                Title = todoItemDto.Title,
                IsCompleted = todoItemDto.IsCompleted,
                //CreatedAt = DateTime.UtcNow,
                TypeId = todoItemDto.TypeId,
                UserId = userId
            };

            _context.TodoItems.Add(todoItemToAdd);   // todoItem вернется с id
            await _context.SaveChangesAsync();  // отправляется запрос к бд

            var todoItemDtoToReturnDto = new TodoItemDto
            {
                Id = todoItemToAdd.Id,
                Title = todoItemToAdd.Title,
                IsCompleted = todoItemToAdd.IsCompleted,
                TypeId = todoItemToAdd.TypeId
            };

            return todoItemDtoToReturnDto;
        }

        //

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
            todoItem.TypeId = updatedTodoItemDto.TypeId;

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

            var todoItemFind = new TodoItemDto
            {
                Title = todoItem.Title,
                IsCompleted = todoItem.IsCompleted,
                Id = todoItem.Id
            };

            return todoItemFind;
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

        public async Task<List<TodoItemForSearchDto>> SearchTodosAsync(DateTime? createdAt = null, int? typeId = null,
            string? authorId = null)
        {
            var query = _context.TodoItems
                .Include(t => t.Type)
                .Include(t => t.User)
                .AsQueryable();


            // фильтрация по дате
            if (createdAt.HasValue)
            {
                var startDate = createdAt.Value.Date.ToUniversalTime();
                var endDate = startDate.AddDays(1).AddTicks(-1);

                query = query.Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

                //var createdAtUtc = DateTime.SpecifyKind(createdAt.Value, DateTimeKind.Utc);
                //query = query.Where(t => t.CreatedAt == createdAtUtc);
            }


            // фильтрация по typeId
            if (typeId.HasValue)
            {
                query = query.Where(t => t.TypeId == typeId.Value);
            }


            // фильтрация по authorId
            if (!string.IsNullOrEmpty(authorId))
            {
                query = query.Where(t => t.UserId == authorId);
            }


            // проекция в DTO
            var todos = await query
                .Select(t => new TodoItemForSearchDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt,
                    TypeName = t.Type != null ? t.Type.Name : "Неизвестно",
                    AuthorName = t.User != null ? t.User.FirstName + " " + t.User.LastName : "Неизвестный автор"
                })
                .ToListAsync();

            return todos;
        }

        public async Task<List<TodoItemForSearchDto>> SearchTodosDapperAsync(DateTime? createdAt = null, int? typeId = null, string? authorId = null)
        {
            _logger.LogInformation("createdAt: {createdAt}", createdAt);
            _logger.LogInformation("id type: {typeId}", typeId);

            // вычисляем диапазон дат, если createdAt передан
            DateTime? startDate = createdAt?.Date.ToUniversalTime();
            DateTime? endDate = startDate?.AddDays(1).AddTicks(-1);

            _logger.LogInformation("startDate: {startDate} endDate: {endDate}", startDate, endDate);


            const string sql = @$"
                SELECT
                    t.""{nameof(TodoItem.Id)}"",
                    t.""{nameof(TodoItem.Title)}"",
                    t.""{nameof(TodoItem.IsCompleted)}"",
                    t.""{nameof(TodoItem.CreatedAt)}"",
                    ty.""{nameof(TypeTodo.Name)}"" AS TypeName,
                    u.""{nameof(ApplicationUser.FirstName)}"" || ' ' || u.""{nameof(ApplicationUser.LastName)}"" AS AuthorName
                FROM
                    ""TodoItems"" t
                JOIN
                    ""TypeTodos"" ty ON t.""TypeId"" = ty.""{nameof(TypeTodo.Id)}""
                JOIN
                    ""AspNetUsers"" u ON t.""UserId"" = u.""{nameof(ApplicationUser.Id)}""
                WHERE
                    (@createdAt IS NULL OR (t.""{nameof(TodoItem.CreatedAt)}"" >= @startDate AND t.""{nameof(TodoItem.CreatedAt)}"" <= @endDate))
                    AND (@typeId IS NULL OR t.""TypeId"" = @typeId)
                    AND (@authorId IS NULL OR t.""UserId"" = @authorId);";

            
            var parameters = new DynamicParameters();

            parameters.Add("@createdAt", createdAt?.ToUniversalTime(), System.Data.DbType.DateTime);
            parameters.Add("@startDate", startDate, System.Data.DbType.DateTime);
            parameters.Add("@endDate", endDate, System.Data.DbType.DateTime);
            parameters.Add("@typeId", typeId, System.Data.DbType.Int32);
            parameters.Add("@authorId", authorId, System.Data.DbType.String);

            _logger.LogInformation("Executing SQL: {sql} with parameters: {parameters}", sql, parameters);


            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new NpgsqlConnection(connectionString))
            {

                var result = await connection.QueryAsync<TodoItemForSearchDto>(sql, parameters);
                return result.ToList();
            }
        }

    }
}
