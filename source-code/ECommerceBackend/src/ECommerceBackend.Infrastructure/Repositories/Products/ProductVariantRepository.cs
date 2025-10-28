using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories.Products;



/// <summary>
/// Repository implementation for managing <see cref="ProductVariant"/> entities.
/// </summary>
public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ProductVariant?> GetBySkuAsync(string sku, Guid shopId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariant>()
            .Include(pv => pv.Product)
            .Where(pv => pv.Sku == sku && pv.Product.ShopId == shopId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariant>()
            .Where(pv => pv.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductVariant>> GetByStatusAsync(VariantStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariant>()
            .Where(pv => pv.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductVariant>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariant>()
            .Where(pv => pv.Price >= minPrice && pv.Price <= maxPrice)
            .OrderBy(pv => pv.Price)
            .ToListAsync(cancellationToken);
    }

    public void Update(ProductVariant productVariant)
    {
        _dbContext.Set<ProductVariant>().Update(productVariant);
    }
}
