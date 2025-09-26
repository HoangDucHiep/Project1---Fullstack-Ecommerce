using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

public enum UserStatus
{
    ACTIVE,
    LOCKED,
    DELETED
}

public sealed class User : Entity
{
    public string IdentityId { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public UserStatus Status { get; private set; }
    // public  string UserType { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Roles list
    // later

    public static User Create(string identityId, string email, string phone)
    {
        // Need to implement roles, events later

        return new User
        {
            IdentityId = identityId,
            Email = email,
            Phone = phone,
            Status = UserStatus.ACTIVE,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }


}
