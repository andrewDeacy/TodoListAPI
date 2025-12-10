using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Repository.Models;
using TodoListAPI.Repository.Repositories;

namespace TodoListAPI.Services.Services;

/// <summary>
/// Service implementation for authentication operations.
/// Handles user registration, login, and JWT token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the AuthService class.
    /// </summary>
    /// <param name="userRepository">Repository for User operations.</param>
    /// <param name="configuration">Configuration for accessing JWT settings.</param>
    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Registers a new user with hashed password.
    /// </summary>
    public async Task<(string Token, Guid UserId, string Email, string Username)> RegisterAsync(RegisterRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
        }

        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");
        }

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = passwordHash,
            Role = "User", // Default role
            CreatedDate = DateTime.UtcNow
        };

        // Save user to database
        var createdUser = await _userRepository.CreateAsync(user);

        // Generate JWT token
        var token = GenerateJwtToken(createdUser);

        return (token, createdUser.Id, createdUser.Email, createdUser.Username);
    }

    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    public async Task<(string Token, Guid UserId, string Email, string Username)> LoginAsync(LoginRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Find user by email or username
        var user = await _userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email/username or password.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email/username or password.");
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);

        return (token, user.Id, user.Email, user.Username);
    }

    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="user">The user for whom to generate the token.</param>
    /// <returns>A JWT token string.</returns>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
