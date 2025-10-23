using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Contracts.Users;

/// <summary>
/// Extension methods for mapping User entities to DTOs
/// </summary>
public static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            IdentityId = user.IdentityId,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            Roles = user.Roles.Select(r => r.Name)
        };
    }
}
