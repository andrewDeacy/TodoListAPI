using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Api.Models.Responses;
using TodoListAPI.Core.Models.Requests;
using TodoListAPI.Services.Services;

namespace TodoListAPI.Api.Controllers;

/// <summary>
/// Controller for authentication operations.
/// Provides endpoints for user registration and login.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// </summary>
    /// <param name="authService">Service for authentication operations.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration request containing email, username, and password.</param>
    /// <returns>The authentication response with JWT token and user information.</returns>
    /// <response code="201">Returns the newly created user with authentication token.</response>
    /// <response code="400">If the request is invalid or email/username is already in use.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        try
        {
            var (token, userId, email, username) = await _authService.RegisterAsync(request);
            
            var response = new AuthResponse
            {
                Token = token,
                UserId = userId,
                Email = email,
                Username = username
            };

            return CreatedAtAction(nameof(Register), response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email/username and password.</param>
    /// <returns>The authentication response with JWT token and user information.</returns>
    /// <response code="200">Returns the authentication token and user information.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Validation is automatically handled by [ApiController] attribute
        // Invalid requests return ProblemDetails (RFC 7807) before this method is called

        try
        {
            var (token, userId, email, username) = await _authService.LoginAsync(request);
            
            var response = new AuthResponse
            {
                Token = token,
                UserId = userId,
                Email = email,
                Username = username
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}
