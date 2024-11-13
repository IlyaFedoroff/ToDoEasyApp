using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ToDoEasyApp.Data;
using ToDoEasyApp.Filters.ExceptionFilters;
using ToDoEasyApp.Filters.IAsyncActionFilters;
using ToDoEasyApp.Models;
using ToDoEasyApp.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<TodoItem_ValidateTodoItemIdIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_ValidateCreateTodoItemIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_ValidateUpdateTodoItemIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter>();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowClientApp",
//        builder => builder.WithOrigins("https://localhost:4200")
//        .AllowAnyHeader()
//        .AllowAnyMethod());
//});

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

//app.UseHttpsRedirection();

app.MapControllers();
app.Run();