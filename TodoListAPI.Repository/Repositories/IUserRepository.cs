using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository interface for User operations.
/// Provides data access methods for User entities.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The User entity if found, null otherwise.</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The User entity if found, null otherwise.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The User entity if found, null otherwise.</returns>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by their email or username.
    /// Useful for login operations where either identifier can be used.
    /// </summary>
    /// <param name="emailOrUsername">The email address or username of the user.</param>
    /// <returns>The User entity if found, null otherwise.</returns>
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The User entity to create.</param>
    /// <returns>The created User entity with generated Id and timestamps.</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Checks if an email address is already in use.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if the email is already in use, false otherwise.</returns>
    Task<bool> EmailExistsAsync(string email);

    /// <summary>
    /// Checks if a username is already in use.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the username is already in use, false otherwise.</returns>
    Task<bool> UsernameExistsAsync(string username);
}
