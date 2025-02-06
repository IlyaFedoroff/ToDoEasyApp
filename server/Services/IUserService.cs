using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using server.Models;
using ToDoEasyApp.Data;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace server.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterModel model);
        Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByCompletedTodosAsync();
        Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByRecentActivityAsync();
        Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByTaskDifferenceAsync();
        Task<List<ApplicationUserDto>> GetUsersAsync();
    }



    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributedCache;

        private const string CacheKeyUsers = "Users";
        private const string CacheKeyCompletedTodos = "UsersWithCompletedTodos";
        private const string CacheKeyRecentActivity = "UsersWithRecentActivity";
        private const string CacheKeyTaskDifference = "UsersWithTaskDifference";

        public UserService(UserManager<ApplicationUser> userManager,
            ILogger<UserService> logger,
            ApplicationDbContext context,
            IMemoryCache cache,
            IDistributedCache distributedCache)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _cache = cache;
            _distributedCache = distributedCache;
        }

        public async Task RegisterAsync(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentException("Обязательные поля не могут быть пустыми.");
            }
            _logger.LogInformation($"FirstName from model: {model.FirstName}, LastName from model: {model.LastName}, Email from model: {model.Email}");
            var user = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, UserName = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Регистрация пользователя не удалась: {errorMessages}");
            }

            // удаление старого кэша
            _cache.Remove(CacheKeyUsers);
            _cache.Remove(CacheKeyCompletedTodos);
            _cache.Remove(CacheKeyRecentActivity);
            _cache.Remove(CacheKeyTaskDifference);

        }

        public async Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByCompletedTodosAsync()
        {
            // пробуем получить данные из Redis
            var cachedData = await _distributedCache.GetStringAsync(CacheKeyCompletedTodos);

            if (!string.IsNullOrEmpty(cachedData))
            {
                // если есть данные в кэше, то десериализуем и возвращаем
                var deserializedData = JsonSerializer.Deserialize<List<ApplicationUserWithTodosDto>>(cachedData);
                if (deserializedData != null)
                {
                    return deserializedData;
                }
            }

            // если нет данных в кэше то делаем запрос к бд
            var usersWithCompletedTodos = await _context.Users
                .Select(user => new ApplicationUserWithTodosDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CompletedTodosCount = user.TodoItems.Count(todo => todo.IsCompleted)
                })
                .OrderByDescending(user => user.CompletedTodosCount)
                .ToListAsync();

            // кэшируем результат на 10 минут
            await _distributedCache.SetStringAsync(CacheKeyCompletedTodos,
                JsonSerializer.Serialize(usersWithCompletedTodos),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return usersWithCompletedTodos;
        }

        public async Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByRecentActivityAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(CacheKeyRecentActivity);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var deserializedData = JsonSerializer.Deserialize<List<ApplicationUserWithTodosDto>>(cachedData);
                if (deserializedData != null)
                {
                    return deserializedData;
                }
            }


            var usersWithRecentActivity = await _context.Users
                .Select(user => new ApplicationUserWithTodosDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    LastActivity = user.TodoItems
                        .OrderByDescending(todo => todo.CreatedAt)
                        .Select(todo => todo.CreatedAt)
                        .FirstOrDefault()   // берем дату последней активности
                })
                .OrderByDescending(user => user.LastActivity)
                .ToListAsync();

            await _distributedCache.SetStringAsync(CacheKeyRecentActivity,
                JsonSerializer.Serialize(usersWithRecentActivity),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return usersWithRecentActivity;
        }


        public async Task<List<ApplicationUserWithTodosDto>> GetUsersSortedByTaskDifferenceAsync()
        {
            var cachedData = await _distributedCache.GetStringAsync(CacheKeyTaskDifference);

            if (!string.IsNullOrEmpty(cachedData))
            {
                var deserializedData = JsonSerializer.Deserialize<List<ApplicationUserWithTodosDto>>(cachedData);
                if (deserializedData != null)
                {
                    return deserializedData;
                }
            }


            var usersWithTaskDifference = await _context.Users
                .Select(user => new ApplicationUserWithTodosDto
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CompletedTodosCount = user.TodoItems.Count(todo => todo.IsCompleted),
                    IncompletedTodosCount = user.TodoItems.Count(todo => !todo.IsCompleted)
                })
                .OrderByDescending(user => user.CompletedTodosCount - user.IncompletedTodosCount)
                .ToListAsync();

            await _distributedCache.SetStringAsync(CacheKeyTaskDifference,
                JsonSerializer.Serialize(usersWithTaskDifference),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });


            return usersWithTaskDifference;
        }



        public async Task<List<ApplicationUserDto>> GetUsersAsync()
        {
            if (!_cache.TryGetValue(CacheKeyUsers, out List<ApplicationUserDto>? users))
            {
                users = await _context.Users
                    .Select(user => new ApplicationUserDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    })
                    .ToListAsync();

                // настройка кэша
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                // сохранение данных в кэше
                _cache.Set(CacheKeyUsers, users, cacheOptions);
            }

            return users ?? new List<ApplicationUserDto>();
        }
    }
}
