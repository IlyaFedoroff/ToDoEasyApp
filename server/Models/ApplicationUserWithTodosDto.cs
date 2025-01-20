namespace server.Models
{
    public class ApplicationUserWithTodosDto
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
            
        public int CompletedTodosCount { get; set; }
    }
}
