using System.Security.Claims;
using ECommerceBackend.Application.Abstracts.Authentication;
using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? PhoneNumber => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.MobilePhone)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
