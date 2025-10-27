namespace ECommerceBackend.Domain.Products;

/// HDHiep - 10/21/2025
/// <summary>
/// Repository interface for managing <see cref="ProductOptionType"/> entities.
/// </summary>
public interface IProductOptionTypeRepository
{
    Task<List<ProductOptionType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductOptionType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductOptionType>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductOptionType?> GetByProductIdAndNameAsync(Guid productId, string name, CancellationToken cancellationToken = default);


    Task AddAsync(ProductOptionType productOptionType, CancellationToken cancellationToken = default);
    void Update(ProductOptionType productOptionType);
    void Delete(ProductOptionType productOptionType);

}
