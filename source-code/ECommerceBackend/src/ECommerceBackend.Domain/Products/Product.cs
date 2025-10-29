using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;

namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Represents the status of a product.
/// </summary>
public enum ProductStatus
{
    Active, // publicly available
    Inactive,   // hidden from customers
    Deleted,  // soft-deleted
    Locked,   // locked by admin
    OutOfStock  // out of stock
}

/// HDHiep - 10/21/2025
/// <summary>
/// Represents a product in the e-commerce system.
/// </summary>
public sealed class Product : Entity
{
    public Guid ShopId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Slug { get; private set; }
    public string Sku { get; private set; } // Product SKU
    public ProductStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    // Navigation properties
    public Category Category { get; private set; } = null!;
    public ICollection<ProductMedia> ProductMedias { get; private set; } = [];
    public ICollection<ProductVariant> ProductVariants { get; private set; } = [];

    public Product()
    {
        // Required by EF Core
    }

    public static Product Create(Guid shopId, Guid categoryId, string name, string description, string slug, string sku, ProductStatus status)
    {
        var product = new Product
        {
            ShopId = shopId,
            CategoryId = categoryId,
            Name = name,
            Description = description,
            Slug = slug,
            Sku = sku,
            Status = status,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // product.Raise(new ProductCreatedEvent(product.Id));
        return product;
    }
}
