using ECommerceBackend.Domain.Shops;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;
public class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Shop?> GetByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Shop>()
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId, cancellationToken);
    }
}
