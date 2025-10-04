using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

/// HDHiep - 09/27/2025
/// <summary>
/// Repository implementation for managing <see cref="User"/> entities.
/// Inherits from the generic <see cref="Repository{T}"/> class and implements the <see cref="IUserRepository"/> interface.
/// Defines methods for retrieving and adding users.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<User>().FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
    }
}
