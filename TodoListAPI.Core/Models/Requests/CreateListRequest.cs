using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Core.Models.Requests;

/// <summary>
/// Request model for creating a new todo list.
/// </summary>
public class CreateListRequest
{
    /// <summary>
    /// Name/title of the todo list. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo list. Maximum 1000 characters.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
}