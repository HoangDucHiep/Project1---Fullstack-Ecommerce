using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories.Products;



/// <summary>
/// Repository implementation for managing <see cref="ProductVariantOptionValue"/> entities.
/// </summary>
public class ProductVariantOptionValueRepository : IProductVariantOptionValueRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductVariantOptionValueRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<List<ProductVariantOptionValue>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariantOptionValue>().ToListAsync(cancellationToken);
    }

    public async Task<ProductVariantOptionValue?> GetByCompositeKeyAsync(Guid variantId, Guid optionValueId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariantOptionValue>()
            .FirstOrDefaultAsync(pvov => pvov.VariantId == variantId && pvov.OptionValueId == optionValueId, cancellationToken);
    }

    public async Task<List<ProductVariantOptionValue>> GetByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariantOptionValue>()
            .Where(pvov => pvov.VariantId == variantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductVariantOptionValue>> GetByOptionValueIdAsync(Guid optionValueId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariantOptionValue>()
            .Where(pvov => pvov.OptionValueId == optionValueId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ProductVariantOptionValue>> GetByOptionValueIdsAsync(List<Guid> optionValueIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ProductVariantOptionValue>()
            .Where(pvov => optionValueIds.Contains(pvov.OptionValueId))
            .ToListAsync(cancellationToken);
    }

    public void Delete(ProductVariantOptionValue productVariantOptionValue)
    {
        _dbContext.Set<ProductVariantOptionValue>().Remove(productVariantOptionValue);
    }

    public async Task DeleteByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        List<ProductVariantOptionValue> entities = await _dbContext.Set<ProductVariantOptionValue>()
            .Where(pvov => pvov.VariantId == variantId)
            .ToListAsync(cancellationToken);

        _dbContext.Set<ProductVariantOptionValue>().RemoveRange(entities);
    }

    public async Task AddAsync(ProductVariantOptionValue productVariantOptionValue, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<ProductVariantOptionValue>().AddAsync(productVariantOptionValue, cancellationToken);
    }

    public void Update(ProductVariantOptionValue productVariantOptionValue)
    {
        _dbContext.Set<ProductVariantOptionValue>().Update(productVariantOptionValue);
    }
}
