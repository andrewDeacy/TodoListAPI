using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Api.Models.Requests;

/// <summary>
/// Request model for updating an existing todo list.
/// </summary>
public class UpdateListRequest
{
    /// <summary>
    /// Updated name/title of the todo list. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Updated description of the todo list. Optional, maximum 1000 characters.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
}