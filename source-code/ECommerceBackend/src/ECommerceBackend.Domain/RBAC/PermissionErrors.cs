using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.RBAC;

public static class PermissionErrors
{
    public static readonly Error NotFound = new("Permission.NotFound", "Permission not found", ErrorType.NotFound);
    public static readonly Error InvalidCode = new("Permission.InvalidCode", "Permission code is invalid", ErrorType.Validation);
}
