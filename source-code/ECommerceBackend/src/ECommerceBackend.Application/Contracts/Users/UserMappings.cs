using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Application.Contracts.Users;

/// <summary>
/// Extension methods for mapping User entities to DTOs
/// </summary>
public static class UserMappings
{
    public static MeUserDto ToDto(this User user)
    {
        return new MeUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            Status = user.Status.ToString(),
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,
            Avatar_Url = user.Avatar_Url,
            Bio = user.Bio,
            IdCardFullName = user.IdCardFullName,
            IdCardNumber = user.IdCardNumber,
            IdCardFullAddress = user.IdCardFullAddress
        };
    }
}
