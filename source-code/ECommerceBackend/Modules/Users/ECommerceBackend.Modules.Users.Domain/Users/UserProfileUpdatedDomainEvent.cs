using ECommerceBackend.Common.Domain;

namespace ECommerceBackend.Modules.Users.Domain.Users;

/// HoangDucHiep - 08/2025
/// <summary>
/// Domain event raised when a user's profile is updated.
/// </summary>
/// <param name="userId"></param>
/// <param name="email"></param>
public sealed class UserProfileUpdatedDomainEvent(Guid userId, string email) : DomainEvent
{
    public Guid UserId { get; init; } = userId;

    public string Email { get; init; } = email;
}
