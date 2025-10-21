using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Commons;

namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Repository interface for managing <see cref="ProductMedia"/> entities.
/// </summary>
public interface IProductMediaRepository
{
    Task<List<ProductMedia>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductMedia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByProductVariantIdAsync(Guid productVariantId, CancellationToken cancellationToken = default);
    Task<List<ProductMedia>> GetByMediaTypeAsync(MediaType mediaType, CancellationToken cancellationToken = default);
    
    void Add(ProductMedia productMedia);
    void Update(ProductMedia productMedia);
    void Delete(ProductMedia productMedia);
}
