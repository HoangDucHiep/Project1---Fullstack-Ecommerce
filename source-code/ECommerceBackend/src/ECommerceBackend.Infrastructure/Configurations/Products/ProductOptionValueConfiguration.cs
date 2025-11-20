using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


/// HDHiep - 10/05/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="ProductOptionValue"/> entity.
/// </summary>
internal sealed class ProductOptionValueConfiguration : IEntityTypeConfiguration<ProductOptionValue>
{
    public void Configure(EntityTypeBuilder<ProductOptionValue> builder)
    {
        builder.ToTable("product_option_values");

        builder.HasKey(pov => pov.Id);

        builder.Property(pov => pov.ProductOptionTypeId)
            .IsRequired();

        builder.Property(pov => pov.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pov => pov.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pov => pov.CreatedAtUtc)
            .IsRequired();

        builder.Property(pov => pov.UpdatedAtUtc)
            .IsRequired();

        // Query filter for soft delete
        builder.HasQueryFilter(pov => !pov.IsDeleted);

        // Indexes
        builder.HasIndex(pov => pov.ProductOptionTypeId);
        builder.HasIndex(pov => new { pov.ProductOptionTypeId, pov.Value }).IsUnique();

        // Relationships
        builder.HasOne<ProductOptionType>()
            .WithMany()
            .HasForeignKey(pov => pov.ProductOptionTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
