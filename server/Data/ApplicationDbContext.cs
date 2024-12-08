namespace ToDoEasyApp.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using server.Models;
    using ToDoEasyApp.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<TodoItem> TodoItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

       
    }

}
