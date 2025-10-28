using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Products;

/// <summary>
/// Represents the status of a product.
/// </summary>
public enum ProductStatus
{
    Active,
    Inactive,
    Deleted,
    Locked,
    OutOfStock
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
    public ProductStatus Status { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public Product()
    {
        // Required by EF Core
    }

    public static Product Create(Guid shopId, Guid categoryId, string name, string description, string slug, ProductStatus status)
    {
        var product = new Product
        {
            ShopId = shopId,
            CategoryId = categoryId,
            Name = name,
            Description = description,
            Slug = slug,
            Status = status,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // product.Raise(new ProductCreatedEvent(product.Id));
        return product;
    }

    public void ChangeCategory(Guid newCategoryId)
    {
        if (newCategoryId == Guid.Empty)
        {
            throw new ArgumentException("New category ID cannot be empty.", nameof(newCategoryId));
        }

        if (newCategoryId == CategoryId)
        {
            return;
        }

        CategoryId = newCategoryId;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

}
