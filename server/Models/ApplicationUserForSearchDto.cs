using System.ComponentModel.DataAnnotations;

namespace server.Models
{
    public class ApplicationUserForSearchDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public required ICollection<TodoItemForSearchDto> TodoItems { get; set; }

    }
}
