using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

/// HDHiep - 10/04/2025
/// <summary>
/// Represents a permission entity in the user management system.
/// </summary>
public class Permission : Entity
{
    public string Name { get; private set; }
    public string Code { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Navigation property through intermediate entity
    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    // Convenience property to get roles
    public IEnumerable<Role> Roles => _rolePermissions.Select(rp => rp.Role);

    public static Permission Create(string name, string code)
    {
        return new Permission
        {
            Name = name,
            Code = code,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
