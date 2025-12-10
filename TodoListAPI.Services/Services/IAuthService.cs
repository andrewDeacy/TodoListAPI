using TodoListAPI.Core.Models.Requests;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service interface for authentication operations.
/// Handles user registration, login, and JWT token generation.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with hashed password.
    /// </summary>
    /// <param name="request">The registration request containing user information.</param>
    /// <returns>A tuple containing the JWT token and user information (UserId, Email, Username).</returns>
    /// <exception cref="InvalidOperationException">Thrown when email or username is already in use.</exception>
    Task<(string Token, Guid UserId, string Email, string Username)> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email/username and password.</param>
    /// <returns>A tuple containing the JWT token and user information (UserId, Email, Username).</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    Task<(string Token, Guid UserId, string Email, string Username)> LoginAsync(LoginRequest request);
}
