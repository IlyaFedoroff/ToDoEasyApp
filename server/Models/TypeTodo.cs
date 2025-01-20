using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoEasyApp.Models;

namespace server.Models
{
    public class TypeTodo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}
