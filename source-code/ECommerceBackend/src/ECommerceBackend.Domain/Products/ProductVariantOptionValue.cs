namespace ECommerceBackend.Domain.Products;



/// HDHiep - 10/21/2025
/// <summary>
/// Represents the association between a product variant and its option values.
/// </summary>
public class ProductVariantOptionValue
{
    public Guid VariantId { get; private set; }
    public Guid OptionValueId { get; private set; }

    public ProductVariantOptionValue()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Creates a new instance of ProductVariantOptionValue.
    /// </summary>
    /// <param name="variantId"> Product Variant ID </param>
    /// <param name="optionValueId"> Product Option Value ID </param>
    /// <returns> New instance of ProductVariantOptionValue </returns>
    public static ProductVariantOptionValue Create(Guid variantId, Guid optionValueId)
    {
        var variantOptionValue = new ProductVariantOptionValue
        {
            VariantId = variantId,
            OptionValueId = optionValueId
        };
        // Raise domain event if needed
        // variantOptionValue.Raise(new ProductVariantOptionValueCreatedEvent(variantOptionValue.VariantId, variantOptionValue.OptionValueId));
        return variantOptionValue;
    }
}
