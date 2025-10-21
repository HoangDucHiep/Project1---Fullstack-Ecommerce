using ECommerceBackend.Domain.Products;
using ECommerceBackend.Domain.Commons;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

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

    public async Task<List<ProductMedia>> GetByMediaTypeAsync(MediaType mediaType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductMedia>()
            .Where(pm => pm.MediaType == mediaType)
            .OrderBy(pm => pm.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public void Update(ProductMedia productMedia)
    {
        _dbContext.Set<ProductMedia>().Update(productMedia);
    }
}
