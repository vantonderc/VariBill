using System.Security.Claims;
using VariBillWebAPI.Services.Interfaces;

namespace VariBillWebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
            }
            return "System";
        }
    }
}
