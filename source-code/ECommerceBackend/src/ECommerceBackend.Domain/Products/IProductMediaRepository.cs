namespace ECommerceBackend.Domain.Products;

public interface IProductMediaRepository
{
    Task<List<ProductMedia>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductMedia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByProductVariantIdAsync(Guid productVariantId, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByMediaIdAsync(Guid mediaId, CancellationToken cancellationToken = default);

    Task AddAsync(ProductMedia productMedia, CancellationToken cancellationToken = default);
    void Update(ProductMedia productMedia);
    void Delete(ProductMedia productMedia);
}
