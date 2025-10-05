using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

/// HDHiep - 10/04/2025
/// <summary>
/// Represents a role entity in the user management system.
/// </summary>
public class Role : Entity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public bool IsSystemRole { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Navigation properties through intermediate entities
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    // Convenience properties
    public IEnumerable<User> Users => _userRoles.Select(ur => ur.User);
    public IEnumerable<Permission> Permissions => _rolePermissions.Select(rp => rp.Permission);

    public static Role Create(string name, string code, bool isSystemRole)
    {
        return new Role
        {
            Name = name,
            Code = code,
            IsSystemRole = isSystemRole,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void AddPermission(Permission permission)
    {
        if (!_rolePermissions.Any(rp => rp.PermissionId == permission.Id))
        {
            _rolePermissions.Add(RolePermission.Create(this, permission));
        }
    }

    public void RemovePermission(Permission permission)
    {
        RolePermission? rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
        }
    }

    public bool HasPermission(Permission permission)
    {
        return _rolePermissions.Any(rp => rp.PermissionId == permission.Id);
    }
}
