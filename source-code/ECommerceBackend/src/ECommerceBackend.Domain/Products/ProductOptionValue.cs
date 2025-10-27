using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Products;


/// HDHiep - 10/21/2025
/// <summary>
/// Represents a value for a product option.
/// </summary>
public class ProductOptionValue : Entity
{
    public Guid ProductOptionTypeId { get; private set; }
    public string Value { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public ProductOptionValue()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new instance of ProductOptionValue.
    /// </summary>
    /// <param name="productOptionTypeId"> Product Option Type ID </param>
    /// <param name="value"> Option value </param>
    /// <returns> New instance of ProductOptionValue </returns>
    public static ProductOptionValue Create(Guid productOptionTypeId, string value)
    {
        var optionValue = new ProductOptionValue
        {
            ProductOptionTypeId = productOptionTypeId,
            Value = value,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // optionValue.Raise(new ProductOptionValueCreatedEvent(optionValue.Id));
        return optionValue;
    }
}
