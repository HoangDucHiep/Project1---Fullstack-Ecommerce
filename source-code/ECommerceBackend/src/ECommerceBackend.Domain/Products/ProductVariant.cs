using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Products;


/// HDHiep - 10/21/2025
/// <summary>
/// Represents the status of a product variant.
/// </summary>
public enum VariantStatus
{
    Active,
    Inactive,
    Deleted,
    Locked,
    OutOfStock
}


/// HDHiep - 10/21/2025
/// <summary>
/// Represents a variant of a product.
/// </summary>
public class ProductVariant : Entity
{
    public Guid ProductId { get; private set; }
    public VariantStatus Status { get; private set; }
    public string Sku { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public double Weight { get; private set; }  // in kilograms
    public double Height { get; private set; }  // Y - in centimeters
    public double Width { get; private set; }   // X - in centimeters
    public double Length { get; private set; }  // Z - in centimeters
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }


    // Navigation property
    public Product Product { get; private set; } = null!;

    public ProductVariant()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new instance of ProductVariant.
    /// </summary>
    /// <param name="productId"> Product ID </param>
    /// <param name="sku"> SKU </param>
    /// <param name="price"> Price </param>
    /// <param name="stock"> Stock quantity </param>
    /// <returns> New instance of ProductVariant </returns>
    public static ProductVariant Create(Guid productId, string sku, decimal price, int stock, double weight, double height, double width, double length)
    {
        var variant = new ProductVariant
        {
            ProductId = productId,
            Sku = sku,
            Price = price,
            Stock = stock,
            Status = VariantStatus.Active,
            Weight = weight,
            Height = height,
            Width = width,
            Length = length,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // variant.Raise(new ProductVariantCreatedEvent(variant.Id));
        return variant;
    }
}
