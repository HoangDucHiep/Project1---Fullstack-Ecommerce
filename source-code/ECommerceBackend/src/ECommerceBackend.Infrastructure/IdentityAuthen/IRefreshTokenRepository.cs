namespace ECommerceBackend.Infrastructure.IdentityAuthen;
public interface IRefreshTokenRepository
{
    Task<List<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken entity);

    void Delete(RefreshToken entity);
    Task<List<RefreshToken>> GetAllByUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllTokensForUserAsync(string identityUserId, CancellationToken cancellationToken = default);
}
