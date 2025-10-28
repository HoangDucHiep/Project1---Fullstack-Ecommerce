namespace ECommerceBackend.Application.Contracts.Products;


/// <summary>
/// DTO for product variant with option combinations
/// </summary>
public record ProductVariantDto(
    List<string> OptionValues,
    decimal Price,
    int Stock,
    string Sku,
    double Weight,
    double Height,
    double Width,
    double Length,
    List<ProductMediaDto>? VariantMedia = null // Ảnh riêng cho variant này
);

