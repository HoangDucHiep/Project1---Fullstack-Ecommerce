using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Api.Controllers.Products;

/// <summary>
/// Request for creating a new product
/// </summary>
public sealed record CreateProductRequest(
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    List<ProductMediaDto> Media,
    List<ProductOptionDto> Options,
    List<ProductVariantDto> Variants
);

