using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


/// HDHiep - 10/05/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="ProductVariantOptionValue"/> entity.
/// </summary>
internal sealed class ProductVariantOptionValueConfiguration : IEntityTypeConfiguration<ProductVariantOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductVariantOptionValue> builder)
    {
        builder.ToTable("product_variant_option_values");

        // Composite primary key
        builder.HasKey(pvov => new { pvov.VariantId, pvov.OptionValueId });

        builder.Property(pvov => pvov.VariantId)
            .IsRequired();

        builder.Property(pvov => pvov.OptionValueId)
            .IsRequired();

        // Indexes
        builder.HasIndex(pvov => pvov.VariantId);
        builder.HasIndex(pvov => pvov.OptionValueId);

        // Relationships
        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(pvov => pvov.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProductOptionValue>()
            .WithMany()
            .HasForeignKey(pvov => pvov.OptionValueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
