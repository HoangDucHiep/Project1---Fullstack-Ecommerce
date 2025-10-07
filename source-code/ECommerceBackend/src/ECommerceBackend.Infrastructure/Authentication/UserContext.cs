using System.Security.Claims;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Domain.Users;
using Microsoft.AspNetCore.Http;

namespace ECommerceBackend.Infrastructure.Authentication;
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public UserContext(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public string? IdentityUserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserId => IdentityUserId is null ? null : _userRepository.GetByIdentityIdAsync(IdentityUserId).GetAwaiter().GetResult()?.Id.ToString();

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? PhoneNumber => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.MobilePhone)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
