using Microsoft.EntityFrameworkCore;

namespace TodoListAPI.Repository;

/// <summary>
/// Database context for TodoList API.
/// Manages database connections and entity configurations.
/// </summary>
public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    // DbSets will be added when entity models are created (Task #2)
    // Example:
    // public DbSet<User> Users { get; set; }
    // public DbSet<TodoList> TodoLists { get; set; }
    // public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity configurations will be added when entity models are created (Task #2)
    }
}

