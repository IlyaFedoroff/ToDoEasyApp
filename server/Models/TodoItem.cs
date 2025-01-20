using server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoEasyApp.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            IsCompleted = false;    //   значение по умолчанию
            CreatedAt = DateTime.UtcNow;
            Title = "standart name";    // стандартный случай когда не получаем Title
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент для Id
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public int TypeId { get; set; }
        public TypeTodo Type { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}



