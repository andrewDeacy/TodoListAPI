using Microsoft.EntityFrameworkCore;
using TodoListAPI.Repository.Models;

namespace TodoListAPI.Repository.Repositories;

/// <summary>
/// Repository implementation for User operations.
/// Provides data access methods for User entities using Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly TodoListDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context for User operations.</param>
    public UserRepository(TodoListDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// Gets a user by their email or username.
    /// Useful for login operations where either identifier can be used.
    /// </summary>
    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var lowerInput = emailOrUsername.ToLower();
        return await _context.Users
            .FirstOrDefaultAsync(u => 
                u.Email.ToLower() == lowerInput || 
                u.Username.ToLower() == lowerInput);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public async Task<User> CreateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Checks if an email address is already in use.
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Checks if a username is already in use.
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }
}
