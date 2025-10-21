using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="ProductVariant"/> entities.
/// </summary>
public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariant>()
            .FirstOrDefaultAsync(pv => pv.Sku == sku, cancellationToken);
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
