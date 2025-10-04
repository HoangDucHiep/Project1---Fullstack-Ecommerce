using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.RBAC;

public sealed class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public string PermissionCode { get; private set; }
    public DateTimeOffset GrantedAtUtc { get; private set; }

    private RolePermission(Guid roleId, string permissionCode)
    {
        RoleId = roleId;
        PermissionCode = permissionCode;
        GrantedAtUtc = DateTimeOffset.UtcNow;
    }

    public static RolePermission Create(Guid roleId, string permissionCode)
    {
        return new RolePermission(roleId, permissionCode);
    }
}
