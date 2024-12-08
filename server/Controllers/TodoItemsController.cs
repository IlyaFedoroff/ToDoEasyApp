namespace ToDoEasyApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using ToDoEasyApp.Models;
    using ToDoEasyApp.Services;
    using Microsoft.Extensions.Logging;
    using System.Security.Claims;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoItemService _todoItemService;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(TodoItemService todoItemService, ILogger<TodoItemsController> logger)
        {
            _todoItemService = todoItemService;
            _logger = logger;

        }

        // GET todoitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItemsAsync()
        {
            _logger.LogInformation("Getting all todo items");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todoItems = await _todoItemService.GetTodoItemsByUserAsync(userId);
            return Ok(todoItems);
        }

        // GET todoitem
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItemByIdAsync(int id)
        {
            try
            {
                var todoItemDto = await _todoItemService.GetTodoItemByIdAsync(id);
                if (todoItemDto == null)
                {
                    return NotFound(new { message = "Не нашли такой todoItem" });
                }
                return Ok(todoItemDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Create a todoItem
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> CreateTodoItemAsync([FromBody] TodoItemDto todoItemDto)
        {
            _logger.LogInformation("Полученные данные: Title={Title}, IsCompleted={IsCompleted}, Id={Id}",
                todoItemDto.Title, todoItemDto.IsCompleted, todoItemDto.Id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todoItem = await _todoItemService.CreateTodoItemAsync(todoItemDto, userId);

            return CreatedAtAction(nameof(GetTodoItemsAsync),
                new { id = todoItemDto.Id },
                todoItemDto);
        }


        // Update todoItem
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItemDto>> PutTodoItemAsync([FromBody] TodoItemDto todoItemDto)
        {
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
        public async Task<IActionResult> DeleteTodoItemAsync(int todoItemId)
        {
            try
            {
                await _todoItemService.DeleteTodoItemAsync(todoItemId);
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
