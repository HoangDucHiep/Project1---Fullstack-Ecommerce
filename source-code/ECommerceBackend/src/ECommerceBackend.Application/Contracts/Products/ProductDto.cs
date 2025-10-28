using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Contracts.Products;


/// <summary>
/// DTO for product information
/// </summary>
public record ProductDto(
    Guid Id,
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Slug,
    ProductStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    List<ProductMediaDto>? Medias = null
)
{
    /// <summary>
    /// Factory method to create ProductDto from Product entity
    /// </summary>
    public static ProductDto FromEntity(Product product, List<ProductMediaDto>? medias = null) => new(
        product.Id,
        product.ShopId,
        product.CategoryId,
        product.Name,
        product.Description,
        product.Slug,
        product.Status,
        product.CreatedAtUtc,
        product.UpdatedAtUtc,
        medias
    );
};
