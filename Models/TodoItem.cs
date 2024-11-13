using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoEasyApp.Models.Validations;

namespace ToDoEasyApp.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            IsCompleted = false;    //   значение по умолчанию
            CreatedAt = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент для Id
        public int Id { get; set; }

        [Required]
        [TodoItem_EnsureTitleIsCorrect]
        public string? Title { get; set; }

        [Required]
        public bool? IsCompleted { get; set; }

        
        public DateTime CreatedAt { get; set; }

    }
}
