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
    using Serilog.Core;
    using ToDoEasyApp.Data;
    using Microsoft.EntityFrameworkCore;

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoItemService _todoItemService;
        private readonly ILogger<TodoItemsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public TodoItemsController(TodoItemService todoItemService, ILogger<TodoItemsController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _todoItemService = todoItemService;
            _logger = logger;
            _userManager = userManager;

            _context = context;

        }

        // GET todoitems
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItemsAsync()
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


        // Create a todoItem
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

            //var userId = User.FindFirst("sub")?.Value;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Полученные данные: Title={Title}, IsCompleted={IsCompleted}, userId={userId}",
                todoItemDto.Title, todoItemDto.IsCompleted, userId);
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


        // Update todoItem
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
        [Route("deleteAll")]
        public async Task<IActionResult> DeleteAllTodoItems()
        {
            await _todoItemService.DeleteAllTodoItemsAsync();
            _logger.LogInformation("Deleted all todoItems");
            return NoContent(); // Возвращаем статус 204 No Content для успешного удаления
        }

    }

}
