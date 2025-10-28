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
    public string UserName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public UserStatus Status { get; private set; }
    // public  string UserType { get; private set; }

    // Additional profile fields
    public string? Avatar_Url { get; private set; }
    public string? Bio { get; private set; }
    public string? IdCardFullName { get; private set; }
    public string? IdCardNumber { get; private set; }
    public string? IdCardFullAddress { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Navigation properties through intermediate entity
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    // Convenience property to get roles
    public IEnumerable<Role> Roles => _userRoles.Select(ur => ur.Role);


    public static User Create(string userName, string identityId, string? email = null, string? phone = null, string? avatar_Url = null, string? bio = null, string? idCardFullName = null, string? idCardNumber = null, string? idCardFullAddress = null)
    {
        // Need to implement roles, events later

        return new User
        {
            IdentityId = identityId,
            UserName = userName,
            Email = email,
            Phone = phone,
            Status = UserStatus.ACTIVE,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            Avatar_Url = avatar_Url,
            Bio = bio,
            IdCardFullName = idCardFullName,
            IdCardNumber = idCardNumber,
            IdCardFullAddress = idCardFullAddress
        };
    }

    public void AddRole(Role role, Guid? assignedBy)
    {
        if (!_userRoles.Any(ur => ur.RoleId == role.Id))
        {
            _userRoles.Add(UserRole.Create(this, role, assignedBy));
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void RemoveRole(Role role)
    {
        UserRole? userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public bool HasRole(Role role)
    {
        return _userRoles.Any(ur => ur.RoleId == role.Id);
    }

}
