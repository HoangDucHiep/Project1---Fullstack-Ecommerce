namespace ECommerceBackend.Domain.RBAC;
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetCustomRolesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Role>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    void Add(Role role);
    void Update(Role role);
    void Delete(Role role);
}
