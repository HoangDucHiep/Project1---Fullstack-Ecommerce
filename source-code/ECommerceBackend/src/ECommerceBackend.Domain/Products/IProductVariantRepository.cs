namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Repository interface for managing <see cref="ProductVariant"/> entities.
/// </summary>
public interface IProductVariantRepository
{
    Task<List<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductVariant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<List<ProductVariant>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<ProductVariant>> GetByStatusAsync(VariantStatus status, CancellationToken cancellationToken = default);
    Task<List<ProductVariant>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    
    void Add(ProductVariant productVariant);
    void Update(ProductVariant productVariant);
    void Delete(ProductVariant productVariant);
}
