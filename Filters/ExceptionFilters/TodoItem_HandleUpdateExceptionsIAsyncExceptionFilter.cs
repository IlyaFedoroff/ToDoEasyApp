using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Services;

namespace ToDoEasyApp.Filters.ExceptionFilters
{
    public class TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter : IAsyncExceptionFilter
    {
        private readonly TodoItemService _todoItemService;

        public TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter(TodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var strTodoItemId = context.RouteData.Values["id"] as string;
            
            if (int.TryParse(strTodoItemId, out int todoItemid)) 
            {
                if (!await _todoItemService.TodoItemExists(todoItemid))
                {
                    context.ModelState.AddModelError("TodoItemId", "TodoItemId does not exist anymore.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                    return;
                }
            }
        }
    }
}
