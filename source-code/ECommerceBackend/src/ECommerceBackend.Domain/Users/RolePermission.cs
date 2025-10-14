using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

/// HDHiep - 10/04/2025
/// <summary>
/// Represents the relationship between Role and Permission with additional metadata
/// </summary>
public class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public string PermissionCode { get; private set; } = null!;
    public DateTimeOffset GrantedAtUtc { get; private set; }

    // Navigation properties
    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;


    private RolePermission() { } // For EF Core

    public static RolePermission Create(Guid roleId, Guid permissionId, string permissionCode)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            PermissionCode = permissionCode,
            GrantedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public static RolePermission Create(Role role, Permission permission)
    {
        return new RolePermission
        {
            Role = role,
            Permission = permission,
            RoleId = role.Id,
            PermissionId = permission.Id,
            GrantedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
