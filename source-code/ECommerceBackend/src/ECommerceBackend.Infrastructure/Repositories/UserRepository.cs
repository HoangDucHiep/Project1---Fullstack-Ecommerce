using ECommerceBackend.Domain.Users;

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
}
