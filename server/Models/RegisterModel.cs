using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения.")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Фамилия обязательна для заполнения.")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Email обязателен для заполнения.")]
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не короче 6 символов.")]
        public string Password { get; set; } = null!;
    }
}
