using System.Text.Json.Serialization;
using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Contracts.Products;

/// <summary>
/// DTO for detailed product information including options, variants, and media
/// </summary>
public record ProductDetailDto(
    Guid Id,
    Guid ShopId,
    Guid CategoryId,
    string Name,
    string Description,
    string Slug,
    string Sku, // Product SKU - always present
    ProductStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    List<ProductMediaDto> Medias,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    List<ProductOptionDto>? Options,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    List<ProductVariantDetailDto>? Variants,

    bool HasVariants,
    int VariantCount,

    // For simple products - only serialize when not null
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Price,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? Stock,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Weight,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Height,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Width,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Length,

    // For complex products (with variants) - price range and total stock
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? MinPrice,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? MaxPrice,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    int? TotalStock
)
{
    /// <summary>
    /// Creates a clean DTO with null values removed based on product type
    /// </summary>
    public static ProductDetailDto CreateClean(
        Guid id,
        Guid shopId,
        Guid categoryId,
        string name,
        string description,
        string slug,
        string sku, // Product SKU
        ProductStatus status,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        List<ProductMediaDto> medias,
        bool hasVariants,
        int variantCount,
        List<ProductOptionDto>? options = null,
        List<ProductVariantDetailDto>? variants = null,
        // Simple product fields
        decimal? price = null,
        int? stock = null,
        decimal? weight = null,
        decimal? height = null,
        decimal? width = null,
        decimal? length = null,
        // Complex product fields
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? totalStock = null)
    {
        return new ProductDetailDto(
            id,
            shopId,
            categoryId,
            name,
            description,
            slug,
            sku, // Product SKU - always present
            status,
            createdAtUtc,
            updatedAtUtc,
            medias,
            hasVariants ? options : null,
            hasVariants ? variants : null,
            hasVariants,
            variantCount,
            // Simple product fields (only for non-variant products)
            hasVariants ? null : price,
            hasVariants ? null : stock,
            hasVariants ? null : weight,
            hasVariants ? null : height,
            hasVariants ? null : width,
            hasVariants ? null : length,
            // Complex product fields (only for variant products)
            hasVariants ? minPrice : null,
            hasVariants ? maxPrice : null,
            hasVariants ? totalStock : null
        );
    }
}

/// <summary>
/// DTO for detailed product variant information
/// </summary>
public record ProductVariantDetailDto(
    Guid Id,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    List<string> OptionValues,

    decimal Price,
    int Stock,
    string? Sku, // Variant SKU
    VariantStatus Status,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Weight,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Height,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Width,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    decimal? Length,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    List<ProductMediaDto> VariantMedias,

    DateTimeOffset CreatedAtUtc
);
