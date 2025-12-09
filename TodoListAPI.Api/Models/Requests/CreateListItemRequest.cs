using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Api.Models.Requests;

/// <summary>
/// Request model for creating a new todo item within a list.
/// </summary>
public class CreateListItemRequest
{
    /// <summary>
    /// Title/name of the todo item. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description/details of the todo item. Maximum 1000 characters.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// Optional due date for the todo item.
    /// </summary>
    public DateTime? DueDate { get; set; }
}