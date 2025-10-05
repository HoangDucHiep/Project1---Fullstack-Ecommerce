using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Products;


public enum ProductStatus
{
    Active,
    Inactive,
    Deleted,
    Locked,
    OutOfStock
}


public sealed class Product : Entity
{
    public Guid ShopId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Slug { get; private set; }
    public ProductStatus Status { get; private set; }
    public string Medias { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public Product()
    {
        // Required by EF Core
    }

    public static Product Create(Guid shopId, Guid categoryId, string name, string description, string slug, ProductStatus status, string medias)
    {
        var product = new Product
        {
            ShopId = shopId,
            CategoryId = categoryId,
            Name = name,
            Description = description,
            Slug = slug,
            Status = status,
            Medias = medias,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // product.Raise(new ProductCreatedEvent(product.Id));
        return product;
    }
}
