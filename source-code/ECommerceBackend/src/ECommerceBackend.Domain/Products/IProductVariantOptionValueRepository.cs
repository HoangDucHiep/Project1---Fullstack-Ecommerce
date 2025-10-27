namespace ECommerceBackend.Domain.Products;


/// <summary>
/// Repository interface for managing <see cref="ProductVariantOptionValue"/> entities.
/// </summary>
public interface IProductVariantOptionValueRepository
{
    Task<List<ProductVariantOptionValue>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductVariantOptionValue?> GetByCompositeKeyAsync(Guid variantId, Guid optionValueId, CancellationToken cancellationToken = default);
    Task<List<ProductVariantOptionValue>> GetByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<List<ProductVariantOptionValue>> GetByOptionValueIdAsync(Guid optionValueId, CancellationToken cancellationToken = default);
    Task AddAsync(ProductVariantOptionValue productVariantOptionValue, CancellationToken cancellationToken = default);
    void Update(ProductVariantOptionValue productVariantOptionValue);
    void Delete(ProductVariantOptionValue productVariantOptionValue);
}
