using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories.Products;


/// <summary>
/// Repository implementation for managing <see cref="ProductMedia"/> entities.
/// </summary>
public class ProductMediaRepository : Repository<ProductMedia>, IProductMediaRepository
{
    public ProductMediaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ProductMedia>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductMedia>()
            .Where(pm => pm.ProductId == productId)
            .OrderBy(pm => pm.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductMedia>> GetByProductVariantIdAsync(Guid productVariantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductMedia>()
            .Where(pm => pm.ProductVariantId == productVariantId)
            .OrderBy(pm => pm.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public void Update(ProductMedia productMedia)
    {
        _dbContext.Set<ProductMedia>().Update(productMedia);
    }

    public async Task<List<ProductMedia>> GetByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductMedia>()
            .Where(pm => pm.MediaId == mediaId)
            .OrderBy(pm => pm.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
