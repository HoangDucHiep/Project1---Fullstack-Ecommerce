namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Repository interface for managing <see cref="Product"/> entities.
/// </summary>
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByShopIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByStatusAsync(ProductStatus status, CancellationToken cancellationToken = default);
    
    void Add(Product product);
    void Update(Product product);
    void Delete(Product product);
}
