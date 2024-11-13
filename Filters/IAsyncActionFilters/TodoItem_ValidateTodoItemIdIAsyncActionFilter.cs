using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Services;

namespace ToDoEasyApp.Filters.IAsyncActionFilters
{
    public class TodoItem_ValidateTodoItemIdIAsyncActionFilter : IAsyncActionFilter
    {
        private readonly TodoItemService _todoItemService;

        public TodoItem_ValidateTodoItemIdIAsyncActionFilter(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var todoItemId = context.ActionArguments["id"] as int?;

            if (todoItemId.HasValue)
            {
                if (todoItemId.Value <= 0)
                {
                    context.ModelState.AddModelError("TodoItemId", "TodoItemId is invalid");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }
                else if (!await _todoItemService.TodoItemExists(todoItemId.Value))
                {
                    context.ModelState.AddModelError("TodoItemId", "TodoItem does not exist");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                    return;
                }
            }

            // если все проверки прошли продолжаем выполнение действия
            await next();
        }
    }
}
