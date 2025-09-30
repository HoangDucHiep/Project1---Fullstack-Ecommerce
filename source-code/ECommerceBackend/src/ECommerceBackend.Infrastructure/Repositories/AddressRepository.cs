using ECommerceBackend.Domain.Addresses;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;
public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Address>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return (List<Address>?)await _dbContext.Set<Address>()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public int Update(Address address)
    {
        return _dbContext.Set<Address>().Update(address).State == EntityState.Modified ? 1 : 0;
    }
}
