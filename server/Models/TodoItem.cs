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
            CreatedAt = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Автоинкремент для Id
        public int Id { get; set; }

        public string? Title { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }


        public string UserId { get; set; }
        //public ApplicationUser User { get; set; }

    }
}



