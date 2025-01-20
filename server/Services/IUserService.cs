﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using server.Models;
using ToDoEasyApp.Data;
using ToDoEasyApp.Models;
namespace server.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterModel model);
        Task<List<ApplicationUserWithTodosDto>> GetUserSortedByCompletedTodosAsync();

        Task<List<ApplicationUserDto>> GetUsersAsync();
    }



    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
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
        }

        public async Task<List<ApplicationUserWithTodosDto>> GetUserSortedByCompletedTodosAsync()
        {
            var usersWithCompletedTodos = await _context.Users
                .Select(user => new ApplicationUserWithTodosDto
                {
                    UserId = user.Id,
                    //UserName = user.UserName,
                    //Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CompletedTodosCount = user.TodoItems.Count(todo => todo.IsCompleted)
                })
                .OrderByDescending(user => user.CompletedTodosCount)
                .ToListAsync();

            return usersWithCompletedTodos;
        }

        public async Task<List<ApplicationUserDto>> GetUsersAsync()
        {
            var users = await _context.Users
                .Select(user => new ApplicationUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                })
                .ToListAsync();

            return users;
        }
    }
}
