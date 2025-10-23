using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product with variants and options
/// </summary>
public sealed record CreateProductCommand(
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    List<ProductMediaDto> Media,
    List<ProductOptionDto> Options,
    List<ProductVariantDto> Variants
) : ICommand<Guid>;

