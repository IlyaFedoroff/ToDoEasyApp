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
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200") // ������� ������ ����� �������
              .AllowAnyHeader() // ��������� ����� ���������
              .AllowAnyMethod()); // ��������� ����� HTTP-������
});

builder.Services.AddControllers();

// ��������� ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����������� identity � DI-����������
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ������� ���������� ��� ������
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6; // ����������� ����� ������
})
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
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Jwt Key is not configured.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();





app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"];
    Console.WriteLine($"Authorization Header: {authHeader}");
    await next();
});

app.MapControllers();


app.Run();