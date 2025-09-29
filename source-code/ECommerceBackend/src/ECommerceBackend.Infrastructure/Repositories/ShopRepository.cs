using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Infrastructure.Repositories;
public class ShopRepository : Repository<Shop>, IShopRepository
{
    public ShopRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
