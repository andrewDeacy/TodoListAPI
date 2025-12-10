using System.ComponentModel.DataAnnotations;

namespace TodoListAPI.Core.Models.Requests;

/// <summary>
/// Request model for user login.
/// Supports login with either email or username.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's email address or username for login.
    /// </summary>
    [Required(ErrorMessage = "Email or username is required.")]
    [MaxLength(256, ErrorMessage = "Email or username cannot exceed 256 characters.")]
    public string EmailOrUsername { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    public string Password { get; set; } = string.Empty;
}
