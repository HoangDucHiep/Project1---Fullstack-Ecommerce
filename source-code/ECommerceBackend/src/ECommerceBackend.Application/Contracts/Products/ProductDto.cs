using ECommerceBackend.Domain.Products;

namespace ECommerceBackend.Application.Contracts.Products;

/// <summary>
/// DTO for product information
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public Guid ShopId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;  // String để map từ SQL
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public List<ProductMediaDto>? Medias { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public long? TotalStock { get; set; }  // Long để match SQL SUM()
    public bool HasVariants { get; set; }

    /// <summary>
    /// Convert Status string back to ProductStatus enum
    /// </summary>
    public ProductStatus GetProductStatus() => Enum.Parse<ProductStatus>(Status);

    /// <summary>
    /// Factory method to create ProductDto from Product entity
    /// </summary>
    public static ProductDto FromEntity(Product product, List<ProductMediaDto>? medias = null) => new()
    {
        Id = product.Id,
        ShopId = product.ShopId,
        CategoryId = product.CategoryId,
        Name = product.Name,
        Description = product.Description,
        Slug = product.Slug,
        Status = product.Status.ToString(),
        CreatedAtUtc = product.CreatedAtUtc,
        UpdatedAtUtc = product.UpdatedAtUtc,
        Medias = medias
    };
}
