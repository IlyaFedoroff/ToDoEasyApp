using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ToDoEasyApp.Data;
using ToDoEasyApp.Filters.ExceptionFilters;
using ToDoEasyApp.Filters.IAsyncActionFilters;
using ToDoEasyApp.Models;
using ToDoEasyApp.Services;
using Serilog;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<TodoItem_ValidateTodoItemIdIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_ValidateCreateTodoItemIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_ValidateUpdateTodoItemIAsyncActionFilter>();
builder.Services.AddScoped<TodoItem_HandleUpdateExceptionsIAsyncExceptionFilter>();


// ��������� Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();  //   ���������� Serilog ��� ������ ��� ����������

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "API ��� ���������� ��������",
        Contact = new OpenApiContact
        {
            Name = "Ilya",
            Email = "alohadance@sobaka.mail.ru"
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
        policy.WithOrigins("http://localhost:4200") // ������� ������ ����� �������
              .AllowAnyHeader() // ��������� ����� ���������
              .AllowAnyMethod()); // ��������� ����� HTTP-������
});

//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.WithOrigins("http://localhost:4200") // ��������� Angular
//              .AllowAnyHeader() // ��������� ����� ���������
//              .AllowAnyMethod(); // ��������� ����� ������ (GET, POST, PUT, DELETE � �.�.)
//    });
//});

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    
}

app.UseCors("AllowClientApp");

//app.UseHttpsRedirection();

app.MapControllers();
app.Run();