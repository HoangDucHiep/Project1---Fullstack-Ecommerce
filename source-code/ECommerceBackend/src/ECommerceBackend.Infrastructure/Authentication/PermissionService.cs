using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Domain.RBAC;
using ECommerceBackend.Domain.Users;

namespace ECommerceBackend.Infrastructure.Authentication;


public class PermissionService : IPermissionService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public PermissionService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return userPermissions.Contains(permissionCode);
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByIdentityIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return Enumerable.Empty<string>();
        }

        var roleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        if (!roleIds.Any())
        {
            return Enumerable.Empty<string>();
        }

        var permissions = new List<string>();
        foreach (Guid roleId in roleIds)
        {
            IEnumerable<string> rolePermissions = await GetRolePermissionsAsync(roleId, cancellationToken);
            permissions.AddRange(rolePermissions);
        }

        return permissions.Distinct();
    }

    public async Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        Role? role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            return Enumerable.Empty<string>();
        }

        return role.RolePermissions.Select(rp => rp.PermissionCode);
    }
}
