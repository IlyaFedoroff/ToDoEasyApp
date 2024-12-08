using System.ComponentModel.DataAnnotations;

namespace ToDoEasyApp.Models
{
    public class TodoItemDto
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public bool? IsCompleted { get; set; }
    }
}
