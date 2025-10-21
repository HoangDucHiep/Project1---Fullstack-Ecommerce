using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Abstracts.Utils;

namespace ECommerceBackend.Domain.Products;

/// HDHiep - 10/21/2025
/// <summary>
/// Represents the types of product options available.
/// </summary>
public class ProductOptionType : Entity
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public ProductOptionType()
    {
        // Required by EF Core
    }


    /// <summary>
    /// Creates a new instance of ProductOptionTypes.
    /// </summary>
    /// <param name="productId"> Product ID </param>
    /// <param name="name"> Option type name </param>
    /// <returns> New instance of ProductOptionTypes </returns>
    public static ProductOptionType Create(Guid productId, string name)
    {
        var optionType = new ProductOptionType
        {
            ProductId = productId,
            Name = name,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        // Raise domain event if needed
        // optionType.Raise(new ProductOptionTypeCreatedEvent(optionType.Id));
        return optionType;
    }
}
