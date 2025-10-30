using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Repositories;

/// HDHiep - 10/20/2025
/// <summary>
/// Repository implementation for managing <see cref="Product"/> entities.
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Product>()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<List<Product>> GetByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.ShopId == shopId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Product>()
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }

    public void Update(Product product)
    {
        _dbContext.Set<Product>().Update(product);
    }

    public async Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<Product>().UpdateRange(products);
        await Task.CompletedTask; 
    }

    public async Task DeleteRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<Product>().RemoveRange(products);
        await Task.CompletedTask;
    }



}
