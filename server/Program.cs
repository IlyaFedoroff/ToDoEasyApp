using Microsoft.EntityFrameworkCore;
using ToDoEasyApp.Data;
using ToDoEasyApp.Services;
using Serilog;
using Microsoft.OpenApi.Models;
using server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using server.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoItemService>();



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

builder.Services.AddControllers();

// ��������� ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� identity � DI-����������
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ��������� JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowClientApp");

//app.UseHttpsRedirection();

app.MapControllers();
app.Run();