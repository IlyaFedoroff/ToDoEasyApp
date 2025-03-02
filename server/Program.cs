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

var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
var postgresConnectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING");
if (string.IsNullOrEmpty(postgresConnectionString))
{
    postgresConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}
Console.WriteLine($"REDIS_CONNECTION_STRING: {redisConnectionString}");
Console.WriteLine($"POSTGRES_CONNECTION_STRING: {postgresConnectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(postgresConnectionString));

builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoItemService>();

builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379";
    options.Configuration = redisConnectionString;
    options.InstanceName = "redis:";
});



Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "API for ToDo Easy App",
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
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://todoeasyapp-angular-client", "http://todoeasyapp-angular-client:80", "http://todoeasyapp-angular-client:4200", "http://localhost", "https://localhost", "http://127.0.0.1:4200" )
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();



builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


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




using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    for (int i = 0; i < 10; i++)
    {
        try
        {
            Console.WriteLine($"Попытка применить миграции ({i + 1}/10)...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Миграции успешно применены!");
            dbContext.SeedData();
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при применении миграций ({i + 1}/10): {ex.Message}");
            await Task.Delay(5000);
        }
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowClientApp");


//app.UseHttpsRedirection();

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
