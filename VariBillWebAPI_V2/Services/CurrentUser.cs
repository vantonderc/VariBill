using System.Security.Claims;
using VariBillWebAPI.Services.Abstractions;
using VariBillWebAPI.Services.Interfaces;

namespace VariBillWebAPI.Services;

/// <summary>
/// Implementation of <see cref="ICurrentUserService"/> using HttpContext.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CurrentUserService"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's ID (sub claim) or "System" if not authenticated.
    /// </summary>
    public string UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                // Try to get the user ID from the "sub" or "nameidentifier" claim
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? user.FindFirst("sub")?.Value;
                return userId ?? "Unknown";
            }
            return "System";
        }
    }
}
