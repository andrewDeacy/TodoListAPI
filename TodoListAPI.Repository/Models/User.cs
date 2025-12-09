namespace TodoListAPI.Repository.Models;

/// <summary>
/// User entity representing an application user.
/// Supports authentication and authorization with role-based access control (RBAC).
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User's email address. Must be unique across all users.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's display name/username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for authentication.
    /// Never store plain text passwords.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's role for RBAC (Role-Based Access Control).
    /// Default value is "User". Other roles could include "Admin", "Moderator", etc.
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Date and time when the user account was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Navigation property to all TodoLists owned by this user.
    /// </summary>
    public ICollection<TodoList> TodoLists { get; set; } = new List<TodoList>();
}
