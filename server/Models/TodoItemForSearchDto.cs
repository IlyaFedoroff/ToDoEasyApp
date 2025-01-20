namespace server.Models
{
    public class TodoItemForSearchDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? TypeName { get; set; } = null!;
        public string? AuthorName { get; set; } = null!;
    }
}
