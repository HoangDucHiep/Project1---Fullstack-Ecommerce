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
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User(string identityId, string email, string phone)
    {
        IdentityId = identityId;
        Email = email;
        Phone = phone;
        Status = UserStatus.ACTIVE;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public static User Create(string identityId, string email, string phone)
    {
        return new User(identityId, email, phone);
    }

    public void AssignRole(Guid roleId, string? assignedBy = null)
    {
        if (_userRoles.Any(ur => ur.RoleId == roleId))
        {
            return; // Role already assigned
        }

        _userRoles.Add(UserRole.Create(Id, roleId, assignedBy));
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void RemoveRole(Guid roleId)
    {
        UserRole? userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void SetRoles(IEnumerable<Guid> roleIds, string? assignedBy = null)
    {
        _userRoles.Clear();
        foreach (Guid roleId in roleIds)
        {
            _userRoles.Add(UserRole.Create(Id, roleId, assignedBy));
        }
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public bool HasRole(Guid roleId)
    {
        return _userRoles.Any(ur => ur.RoleId == roleId);
    }

    public void UpdateInfo(string email, string phone)
    {
        Email = email;
        Phone = phone;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Lock()
    {
        Status = UserStatus.LOCKED;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Unlock()
    {
        Status = UserStatus.ACTIVE;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        Status = UserStatus.DELETED;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
