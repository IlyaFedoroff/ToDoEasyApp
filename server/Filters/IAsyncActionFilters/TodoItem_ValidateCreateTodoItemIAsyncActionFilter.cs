using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Models;
using ToDoEasyApp.Services;

namespace ToDoEasyApp.Filters.IAsyncActionFilters
{
    public class TodoItem_ValidateCreateTodoItemIAsyncActionFilter : IAsyncActionFilter
    {
        private readonly TodoItemService _todoItemService;
        private readonly ILogger<TodoItem_ValidateCreateTodoItemIAsyncActionFilter> _logger;

        public TodoItem_ValidateCreateTodoItemIAsyncActionFilter(TodoItemService todoItemService, ILogger<TodoItem_ValidateCreateTodoItemIAsyncActionFilter> logger)
        {
            _todoItemService = todoItemService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var todoItemDto = context.ActionArguments["todoItemDto"] as TodoItemDto;

            if (todoItemDto == null)
            {
                context.ModelState.AddModelError("TodoItem", "TodoItem object is null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                _logger.LogWarning("TodoItem object is null");
                return;
            }
            else
            {
                var existingTodoItemDto = await _todoItemService.GetTodoItemByProperties(todoItemDto.Title, todoItemDto.IsCompleted);
                if (existingTodoItemDto != null)
                {
                    context.ModelState.AddModelError("TodoItem", "TodoItem already exists");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    _logger.LogWarning("TodoItem already exists");
                    return;

                }
            }
            await next();
        }
    }
}