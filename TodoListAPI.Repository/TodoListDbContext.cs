using Microsoft.EntityFrameworkCore;
using TodoListAPI.Repository.Models;

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

    /// <summary>
    /// DbSet for User entities.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// DbSet for TodoList entities.
    /// </summary>
    public DbSet<TodoList> TodoLists { get; set; }

    /// <summary>
    /// DbSet for TodoItem entities.
    /// </summary>
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
            entity.Property(e => e.CreatedDate).IsRequired();
        });

        // Configure TodoList entity
        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsCompleted).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.UpdatedDate).IsRequired();
            entity.Property(e => e.UserId).IsRequired();

            // Configure relationship with User
            entity.HasOne(e => e.User)
                .WithMany(u => u.TodoLists)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete their lists

            // Configure relationship with TodoItems
            entity.HasMany(e => e.TodoItems)
                .WithOne(i => i.TodoList)
                .HasForeignKey(i => i.ListId)
                .OnDelete(DeleteBehavior.Cascade); // If list is deleted, delete its items
        });

        // Configure TodoItem entity
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ListId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsCompleted).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.Property(e => e.UpdatedDate).IsRequired();
            entity.Property(e => e.Order).IsRequired();
            // DueDate is optional (nullable), no configuration needed

            // Create composite index on (ListId, Order) for efficient ordering queries
            entity.HasIndex(e => new { e.ListId, e.Order });

            // Configure relationship with TodoList
            entity.HasOne(e => e.TodoList)
                .WithMany(l => l.TodoItems)
                .HasForeignKey(e => e.ListId)
                .OnDelete(DeleteBehavior.Cascade); // Already configured in TodoList, but explicit for clarity
        });
    }
}

