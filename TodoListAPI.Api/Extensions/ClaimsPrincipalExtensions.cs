using System.Security.Claims;

namespace TodoListAPI.Api.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to extract user information from JWT claims.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the JWT claims.
    /// </summary>
    /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
    /// <returns>The user ID as a Guid if found, null otherwise.</returns>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) 
            ?? principal.FindFirst("sub"); // JWT standard claim

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Gets the user ID from the JWT claims, throwing an exception if not found.
    /// Use this when the user must be authenticated (after [Authorize] attribute).
    /// </summary>
    /// <param name="principal">The claims principal (typically from HttpContext.User).</param>
    /// <returns>The user ID as a Guid.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID cannot be extracted from claims.</exception>
    public static Guid GetUserIdRequired(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User ID not found in authentication token.");
        }

        return userId.Value;
    }
}
