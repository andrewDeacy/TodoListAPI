namespace TodoListAPI.Repository.Models;

/// <summary>
/// TodoList entity representing a collection of todo items.
/// Each list belongs to a user and can contain multiple todo items.
/// </summary>
public class TodoList
{
    /// <summary>
    /// Unique identifier for the todo list.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name/title of the todo list.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo list.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the entire list is marked as completed.
    /// Default value is false.
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Date and time when the todo list was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Date and time when the todo list was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// Foreign key to the User who owns this todo list.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the User who owns this todo list.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Navigation property to all TodoItems in this list.
    /// </summary>
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
