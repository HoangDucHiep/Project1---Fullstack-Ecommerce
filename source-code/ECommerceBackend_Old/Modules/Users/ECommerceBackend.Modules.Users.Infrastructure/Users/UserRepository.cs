using ECommerceBackend.Modules.Users.Domain.Users;
using ECommerceBackend.Modules.Users.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Modules.Users.Infrastructure.Users;
internal sealed class UserRepository : IUserRepository
{

    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public void Insert(User user)
    {
        _context.Users.Add(user);
    }
}
