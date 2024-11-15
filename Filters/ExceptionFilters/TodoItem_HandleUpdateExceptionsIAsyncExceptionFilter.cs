﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoEasyApp.Services;

namespace ToDoEasyApp.Filters.ExceptionFilters
{
    public class TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter : IAsyncExceptionFilter
    {
        private readonly TodoItemService _todoItemService;
        private readonly ILogger<TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter> _logger;

        public TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter(TodoItemService todoItemService, ILogger<TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter> logger)
        {
            _todoItemService = todoItemService;
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var strTodoItemDtoId = context.RouteData.Values["id"] as string;
            
            if (int.TryParse(strTodoItemDtoId, out int todoItemDtoId)) 
            {
                if (!await _todoItemService.TodoItemExists(todoItemDtoId))
                {
                    context.ModelState.AddModelError("TodoItemId", "TodoItemId does not exist anymore.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result = new NotFoundObjectResult(problemDetails);
                    _logger.LogWarning($"TodoItemId {todoItemDtoId} does not exist anymore");
                    return;
                }
            }
        }
    }
}
