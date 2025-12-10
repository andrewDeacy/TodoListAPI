namespace TodoListAPI.Repository.Models;

/// <summary>
/// TodoItem entity representing a single task/item within a todo list.
/// Each item belongs to a specific TodoList.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// Unique identifier for the todo item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the TodoList that contains this item.
    /// </summary>
    public Guid ListId { get; set; }

    /// <summary>
    /// Title/name of the todo item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description/details of the todo item.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether the todo item is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Date and time when the todo item was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Date and time when the todo item was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// Optional due date for the todo item.
    /// Null if no due date is set.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Order/position of the todo item within its list.
    /// Used for custom ordering and reordering functionality.
    /// Lower values appear first in the list.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Navigation property to the TodoList that contains this item.
    /// </summary>
    public TodoList TodoList { get; set; } = null!;
}
