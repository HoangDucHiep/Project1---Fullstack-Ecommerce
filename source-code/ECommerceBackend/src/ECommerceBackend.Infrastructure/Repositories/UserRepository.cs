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

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdentityIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.IdentityId == identityUserId, cancellationToken);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Phone == phoneNumber, cancellationToken);
    }
}
