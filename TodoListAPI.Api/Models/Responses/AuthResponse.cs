namespace TodoListAPI.Api.Models.Responses;

/// <summary>
/// Response model for authentication operations (login/register).
/// Contains the JWT token and user information.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT token for authenticated requests.
    /// Include this token in the Authorization header as: "Bearer {Token}"
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier of the authenticated user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's username.
    /// </summary>
    public string Username { get; set; } = string.Empty;
}
