namespace TodoListAPI.Api.Models.Responses;

/// <summary>
/// Response model for TodoList returned by API endpoints.
/// Contains only the properties exposed to API consumers.
/// </summary>
public class TodoListResponse
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
    /// Date and time when the todo list was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Date and time when the todo list was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// Collection of TodoItems in this list.
    /// May be empty if the list has no items or if items were not loaded.
    /// </summary>
    public List<TodoItemResponse> Items { get; set; } = new List<TodoItemResponse>();
}
