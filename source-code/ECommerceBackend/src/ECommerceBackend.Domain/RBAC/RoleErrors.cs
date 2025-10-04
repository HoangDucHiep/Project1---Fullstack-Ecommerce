using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.RBAC;

public static class RoleErrors
{
    public static readonly Error NotFound = new("Role.NotFound", "Role not found", ErrorType.NotFound);
    public static readonly Error CodeAlreadyExists = new("Role.CodeExists", "Role code already exists", ErrorType.Conflict);
    public static readonly Error CannotModifySystemRole = new("Role.CannotModifySystem", "Cannot modify system roles", ErrorType.Validation);
    public static readonly Error CannotDeleteSystemRole = new("Role.CannotDeleteSystem", "Cannot delete system roles", ErrorType.Validation);
    public static readonly Error InvalidPermission = new("Role.InvalidPermission", "Invalid permission code", ErrorType.Validation);
}
