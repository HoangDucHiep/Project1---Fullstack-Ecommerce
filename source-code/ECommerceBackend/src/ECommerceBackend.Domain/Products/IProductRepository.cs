namespace ECommerceBackend.Domain.Products;

/// HDHiep - 10/21/2025
/// <summary>
/// Repository interface for managing <see cref="Product"/> entities.
/// </summary>
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, Guid shopId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);

    void Delete(Product product);

}
