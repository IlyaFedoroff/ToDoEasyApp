namespace ToDoEasyApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ToDoEasyApp.Filters.ExceptionFilters;
    using ToDoEasyApp.Filters.IAsyncActionFilters;
    using ToDoEasyApp.Models;
    using ToDoEasyApp.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        // services configs client git dto
        private readonly TodoItemService _todoItemService;

        public TodoItemsController(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        // GET todoitems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItems()
        {

            var todoItems = await _todoItemService.GetAllTodoItems();
            return Ok(todoItems);
        }

        // GET todoitem
        [ServiceFilter(typeof(TodoItem_ValidateTodoItemIdIAsyncActionFilter))]
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDto>> GetTodoItemById(int id)
        {
            var todoItemDto = await _todoItemService.GetTodoItem(id);

            return Ok(todoItemDto);
        }

        // Create a todoItem
        [HttpPost]
        [ServiceFilter(typeof(TodoItem_ValidateCreateTodoItemIAsyncActionFilter))]
        public async Task<ActionResult<TodoItemDto>> PostTodoItem([FromBody] TodoItemDto todoItemDto)
        {
            await _todoItemService.AddTodoItem(todoItemDto);

            return CreatedAtAction(nameof(GetTodoItemById),
                new { id = todoItemDto.Id },
                todoItemDto);
        }


        // Update todoItem
        [ServiceFilter(typeof(TodoItem_ValidateTodoItemIdIAsyncActionFilter))]
        [ServiceFilter(typeof(TodoItem_ValidateUpdateTodoItemIAsyncActionFilter))]
        [ServiceFilter(typeof(TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter))]   // этот фильтр не срабатывает (должен работать когда во время обновления кто-то удалил объект todoItem)
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> PutTodoItem(int id, [FromBody] TodoItemDto todoItemDto)
        {
            var updatedTodoItemDto = await _todoItemService.UpdateTodoItem(todoItemDto, id);
            //return Ok(updatedTodoItem);
            return NoContent();
        }


        // DELETE todoitem
        [ServiceFilter(typeof(TodoItem_ValidateTodoItemIdIAsyncActionFilter))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItemDto = await _todoItemService.GetTodoItem(id);
            await _todoItemService.DeleteTodoItem(id);

            return Ok(todoItemDto);
        }

        [HttpDelete]
        [Route("deleteAll")]
        public async Task<IActionResult> DeleteAllTodoItems()
        {
            await _todoItemService.DeleteAllTodoItems();
            return NoContent(); // Возвращаем статус 204 No Content для успешного удаления
        }

    }

}
