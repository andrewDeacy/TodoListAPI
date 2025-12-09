namespace TodoListAPI.Core.DTOs;

/// <summary>
/// Data Transfer Object for TodoList.
/// Used for transferring data between Repository, Service, and Controller layers.
/// Does not include navigation properties or EF Core-specific features.
/// </summary>
public class TodoListDto
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
    /// </summary>
    public bool IsCompleted { get; set; }

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
    /// Collection of TodoItems in this list (optional, may be null if not loaded).
    /// </summary>
    public List<TodoItemDto>? Items { get; set; }
}

