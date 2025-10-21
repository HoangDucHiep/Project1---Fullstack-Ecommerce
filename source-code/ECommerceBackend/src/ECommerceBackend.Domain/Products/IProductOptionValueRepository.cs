namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Repository interface for managing <see cref="ProductOptionValue"/> entities.
/// </summary>
public interface IProductOptionValueRepository
{
    Task<List<ProductOptionValue>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductOptionValue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductOptionValue>> GetByProductOptionTypeIdAsync(Guid productOptionTypeId, CancellationToken cancellationToken = default);
    Task<ProductOptionValue?> GetByProductOptionTypeIdAndValueAsync(Guid productOptionTypeId, string value, CancellationToken cancellationToken = default);
    
    void Add(ProductOptionValue productOptionValue);
    void Update(ProductOptionValue productOptionValue);
    void Delete(ProductOptionValue productOptionValue);
}
