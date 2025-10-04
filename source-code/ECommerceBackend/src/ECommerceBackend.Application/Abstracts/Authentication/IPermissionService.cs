namespace ECommerceBackend.Application.Abstracts.Authentication;


public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string userId, string permissionCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}
