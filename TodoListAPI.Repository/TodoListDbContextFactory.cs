using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TodoListAPI.Repository;

/// <summary>
/// Design-time factory for creating TodoListDbContext instances during migrations.
/// This is required by EF Core tools to create the DbContext at design time.
/// </summary>
public class TodoListDbContextFactory : IDesignTimeDbContextFactory<TodoListDbContext>
{
    /// <summary>
    /// Creates a new instance of TodoListDbContext for design-time operations (migrations).
    /// Uses SQLite with a default connection string.
    /// </summary>
    public TodoListDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoListDbContext>();
        optionsBuilder.UseSqlite("Data Source=todolist.db");

        return new TodoListDbContext(optionsBuilder.Options);
    }
}

