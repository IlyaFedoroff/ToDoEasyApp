using Microsoft.AspNetCore.Identity;
using ToDoEasyApp.Models;

namespace server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        // навигационное свойство для связи с TodoItem
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
