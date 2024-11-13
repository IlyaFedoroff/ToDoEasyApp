using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Services;
using ToDoEasyApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ToDoEasyApp.Filters.IAsyncActionFilters
{
    public class TodoItem_ValidateUpdateTodoItemIAsyncActionFilter : IAsyncActionFilter
    {
        private readonly TodoItemService _todoItemService;

        public TodoItem_ValidateUpdateTodoItemIAsyncActionFilter(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var id = context.ActionArguments["id"] as int?;
            var todoItem = context.ActionArguments["todoItem"] as TodoItem;

            if (id.HasValue && todoItem != null && id != todoItem.Id)
            {
                context.ModelState.AddModelError("TodoItemId", "TodoItemId is not the same as id");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            await next();
        }

    }
}
