namespace ToDoEasyApp.Data
{
    
    using Microsoft.EntityFrameworkCore;
    using ToDoEasyApp.Models;

    public class ApplicationDbContext : DbContext
    {
        // свойства параметризированного типа
        public DbSet<TodoItem> TodoItems { get; set; }


        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=todoapp;Username=postgres;Password=Lapka");
                optionsBuilder.EnableSensitiveDataLogging();
                //optionsBuilder.UseLazyLodaingProxies();
            }
        }
    }

}
