namespace server.Models
{
    public class TypeTodoForSearchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public required ICollection<TodoItemForSearchDto> TodoItems { get; set; }
    }
}
