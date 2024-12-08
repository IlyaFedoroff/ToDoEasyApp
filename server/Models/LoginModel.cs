using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
