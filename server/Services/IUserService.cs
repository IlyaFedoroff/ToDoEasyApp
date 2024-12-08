using Microsoft.AspNetCore.Identity;
using server.Models;
namespace server.Services
{
    public interface IUserService
    {
        Task RegisterAsync(RegisterModel model);
    }



    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _logger = logger;
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
    }
}
