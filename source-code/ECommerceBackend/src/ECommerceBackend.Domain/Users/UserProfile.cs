using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

/// HDHiep - 10/04/2025
/// <summary>
/// Represents a user profile entity containing additional user information.
/// </summary>
public class UserProfile : Entity
{
    public Guid UserId { get; private set; }
    public string Avatar_Url { get; private set; }
    public string Bio { get; private set; }
    public string IdCardFullName { get; private set; }
    public string IdCardNumber { get; private set; }
    public string IdCardFullAddress { get; private set; }
}
