using ECommerceBackend.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.IdentityAuthen;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _dbContext;

    public RefreshTokenRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(RefreshToken entity)
    {
        await _dbContext.Set<RefreshToken>().AddAsync(entity);

    }

    public void Delete(RefreshToken entity)
    {
        _dbContext.Set<RefreshToken>().Remove(entity);
    }

    public Task<List<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<RefreshToken>> GetAllByUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RefreshToken>().Where(rt => rt.IdentityUserId == identityUserId).ToListAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<RefreshToken>().FindAsync(new object[] { id }, cancellationToken).AsTask();
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<RefreshToken>().FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task RevokeAllTokensForUserAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        List<RefreshToken> userTokens = await _dbContext.Set<RefreshToken>()
            .Where(rt => rt.IdentityUserId == identityUserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (RefreshToken token in userTokens)
        {
            token.IsRevoked = true;
        }
    }
}
