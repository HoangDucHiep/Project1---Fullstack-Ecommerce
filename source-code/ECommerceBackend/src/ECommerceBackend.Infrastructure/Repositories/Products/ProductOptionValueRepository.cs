using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories.Products;



/// <summary>
/// Repository implementation for managing <see cref="ProductOptionValue"/> entities.
/// </summary>
public class ProductOptionValueRepository : Repository<ProductOptionValue>, IProductOptionValueRepository
{
    public ProductOptionValueRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ProductOptionValue>> GetByProductOptionTypeIdAsync(Guid productOptionTypeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductOptionValue>()
            .Where(pov => pov.ProductOptionTypeId == productOptionTypeId)
            .OrderBy(pov => pov.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductOptionValue>> GetByProductOptionTypeIdsAsync(List<Guid> productOptionTypeIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductOptionValue>()
            .Where(pov => productOptionTypeIds.Contains(pov.ProductOptionTypeId))
            .OrderBy(pov => pov.ProductOptionTypeId)
            .ThenBy(pov => pov.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductOptionValue?> GetByProductOptionTypeIdAndValueAsync(Guid productOptionTypeId, string value, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductOptionValue>()
            .FirstOrDefaultAsync(pov => pov.ProductOptionTypeId == productOptionTypeId && pov.Value == value, cancellationToken);
    }

    public void Update(ProductOptionValue productOptionValue)
    {
        _dbContext.Set<ProductOptionValue>().Update(productOptionValue);
    }
}
