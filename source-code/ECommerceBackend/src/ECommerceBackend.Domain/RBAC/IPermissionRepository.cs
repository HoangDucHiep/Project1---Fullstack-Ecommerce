namespace ECommerceBackend.Domain.RBAC;

public interface IPermissionRepository
{
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);
}
