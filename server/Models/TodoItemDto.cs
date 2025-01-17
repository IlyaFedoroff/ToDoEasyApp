using System.ComponentModel.DataAnnotations;

namespace ToDoEasyApp.Models
{
    public class TodoItemDto
    {
       
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public bool IsCompleted { get; set; }
    }
}
