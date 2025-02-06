namespace ToDoEasyApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ToDoEasyApp.Models;
    using ToDoEasyApp.Services;
    using Microsoft.Extensions.Logging;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using server.Models;
    using Npgsql;
    using Serilog.Core;
    using ToDoEasyApp.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoItemService _todoItemService;
        private readonly ILogger<TodoItemsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "TodoTypes";


        public TodoItemsController(TodoItemService todoItemService,
            ILogger<TodoItemsController> logger,
            UserManager<ApplicationUser> userManager,
            IMemoryCache cache)
        {
            _todoItemService = todoItemService;
            _logger = logger;
            _userManager = userManager;
            _cache = cache;
        }

        // GET todoitems
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItemsByUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Получили userId={userId}", userId);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID not found in claims.");
                return Unauthorized();
            }

            var todoItems = await _todoItemService.GetTodoItemsByUserAsync(userId);
            _logger.LogInformation("Getting all todo items for user {UserId}", userId);
            return Ok(todoItems);
        }

        // GET all todoitems
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<TodoItemForSearchDto>>> GetAllTodoItemsAsync()
        {
            _logger.LogInformation("");


            var todoItemsForSearch = await _todoItemService.GetAllTodoItemsAsync();

            return Ok(todoItemsForSearch);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItemAsync(int id)
        {
            _logger.LogInformation("Мы попали сюда :) и такой тут id: {id}", id);
            try
            {
                var todoItemDto = await _todoItemService.GetTodoItemByIdAsync(id);
                if (todoItemDto == null)
                {
                    return NotFound(new { message = "Не нашли такой todoItemDto" });
                }
                return Ok(todoItemDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // получить все типы
        [HttpGet("types")]
        public async Task<ActionResult<TypeTodoDto>> GetTodoTypesAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<TypeTodoDto>? todoTypes))
            {
                // данные отсутствуют в кэше, получаем их из сервиса
                todoTypes = await _todoItemService.GetTypesAsync() ?? Enumerable.Empty<TypeTodoDto>();

                // настройка кэша
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(CacheKey, todoTypes, cacheOptions);
            }

            return Ok(todoTypes);
        }

        // 


        // Create a todoItem
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> PostTodoItemAsync(TodoItemDto todoItemDto)
        {

            // Валидация модели
            if (todoItemDto == null)
            {
                return BadRequest("Данные не могут быть пустыми.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Полученные данные: Title={Title}, IsCompleted={IsCompleted}, userId={userId}, typeId={typeId}",
                todoItemDto.Title, todoItemDto.IsCompleted, userId, todoItemDto.TypeId);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // проверяем, что пользователь существует
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            try
            {
                var addedTodoItemDto = await _todoItemService.AddTodoItemAsync(todoItemDto, userId);
                //return CreatedAtAction(nameof(GetTodoItemAsync), new { id = addedTodoItemDto.Id }, addedTodoItemDto); не понимаю почему не работает :(
                return StatusCode(StatusCodes.Status201Created, addedTodoItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании TodoItem: {Message}", ex.Message);
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }
        }

        // Update todoItem
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItemDto>> PutTodoItemAsync(int id, [FromBody] TodoItemDto todoItemDto)
        {
            if (id != todoItemDto.Id)
            {
                return BadRequest(new { message = "ID в маршруте и теле запроса не соответствуют." });
            }

            try
            {
                var updatedTodoItemDto = await _todoItemService.UpdateTodoItemAsync(todoItemDto);
                if (updatedTodoItemDto == null)
                {
                    return NotFound(new { message = "Не нашли такой todoItem" });
                }
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE todoitem
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItemAsync(int id)
        {
            _logger.LogInformation($"Получили номер id для удаления:{id}", id);
            try
            {
                await _todoItemService.DeleteTodoItemAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("deleteAll")]
        public async Task<IActionResult> DeleteAllTodoItems()
        {
            await _todoItemService.DeleteAllTodoItemsAsync();
            _logger.LogInformation("Deleted all todoItems");
            return NoContent(); // Возвращаем статус 204 No Content для успешного удаления
        }


        // найти задачи по дате создания, названию, автору.
        [HttpGet("search")]
        public async Task<ActionResult<List<TodoItemForSearchDto>>> SearchTodosAsync(
            [FromQuery] DateTime? createdAt = null,
            [FromQuery] int? typeId = null,
            [FromQuery] string? authorId = null)
        {
            var todos = await _todoItemService.SearchTodosAsync(createdAt, typeId, authorId);         // ef
            return Ok(todos);
        }

        // найти задачи по дате создания, названию, автору.
        [HttpGet("search-dapper")]
        public async Task<ActionResult<List<TodoItemForSearchDto>>> SearchTodosDapperAsync(
            [FromQuery] DateTime? createdAt = null,
            [FromQuery] int? typeId = null,
            [FromQuery] string? authorId = null)
        {
            var todos = await _todoItemService.SearchTodosDapperAsync(createdAt, typeId, authorId);     // dapper
            return Ok(todos);
        }

    }

}
