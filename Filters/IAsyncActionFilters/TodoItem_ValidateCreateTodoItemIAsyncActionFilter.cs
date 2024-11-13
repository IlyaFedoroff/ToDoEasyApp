using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Models;
using ToDoEasyApp.Services;

namespace ToDoEasyApp.Filters.IAsyncActionFilters
{
    public class TodoItem_ValidateCreateTodoItemIAsyncActionFilter : IAsyncActionFilter
    {
        private readonly TodoItemService _todoItemService;

        public TodoItem_ValidateCreateTodoItemIAsyncActionFilter(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var todoItem = context.ActionArguments["todoItem"] as TodoItem;

            if (todoItem == null)
            {
                context.ModelState.AddModelError("TodoItem", "TodoItem object is null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }
            else
            {
                var existingTodoItem = await _todoItemService.GetTodoItemByProperties(todoItem.Title, todoItem.IsCompleted);
                if (existingTodoItem != null)
                {
                    context.ModelState.AddModelError("TodoItem", "TodoItem already exists");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;

                }
            }
            await next();
        }
    }
}