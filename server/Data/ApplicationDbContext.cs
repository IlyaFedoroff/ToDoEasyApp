    namespace ToDoEasyApp.Data
    {
        using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore;
        using server.Models;
        using ToDoEasyApp.Models;

        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public DbSet<TodoItem> TodoItems { get; set; }
            public DbSet<TypeTodo> TypeTodos { get; set; }

            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Настройка связи между TodoItem и ApplicationUser
                modelBuilder.Entity<TodoItem>()
                    .HasOne(t => t.User)    // У TodoItem есть один пользователь
                    .WithMany(u => u.TodoItems) // У пользователя много TodoItem
                    .HasForeignKey(t => t.UserId)   // Внешний ключ - UserId
                    .OnDelete(DeleteBehavior.Cascade);  // Удаление пользователя удаляет все его TodoItem

            // Настройка связи между TodoItem и TypeTodo
            modelBuilder.Entity<TodoItem>()
                .HasOne(t => t.Type)    // У TodoItem есть один тип
                .WithMany()             // у Типа может быть много TodoItem
                .HasForeignKey(t => t.TypeId)   // Внешний ключ - TypeId
                .OnDelete(DeleteBehavior.Restrict); // Удаления типа не удаляет связанные TodoItem

                // Дополнительные настройки
                modelBuilder.Entity<TodoItem>()
                    .Property(t => t.CreatedAt)
                    .HasDefaultValueSql("NOW()");   // Используем HasDefaultValueSql для SQL-функций

                modelBuilder.Entity<TodoItem>()
                    .Property(t => t.IsCompleted)
                    .HasDefaultValue(false);    // Установка значения по умолчанию для IsCompleted
            }
        }
    }