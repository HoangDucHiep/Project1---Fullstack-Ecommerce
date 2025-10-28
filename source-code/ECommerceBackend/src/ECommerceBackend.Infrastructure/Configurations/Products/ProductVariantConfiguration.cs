using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


/// HDHiep - 10/05/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="ProductVariant"/> entity.
/// </summary>
internal sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.ProductId)
            .IsRequired();

        builder.Property(pv => pv.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(pv => pv.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pv => pv.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(pv => pv.Stock)
            .IsRequired();

        builder.Property(pv => pv.Weight)
            .IsRequired()
            .HasColumnType("double precision");

        builder.Property(pv => pv.Height)
            .IsRequired()
            .HasColumnType("double precision");

        builder.Property(pv => pv.Width)
            .IsRequired()
            .HasColumnType("double precision");

        builder.Property(pv => pv.Length)
            .IsRequired()
            .HasColumnType("double precision");

        builder.Property(pv => pv.CreatedAtUtc)
            .IsRequired();

        builder.Property(pv => pv.UpdatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(pv => pv.ProductId);
        builder.HasIndex(pv => pv.Status);
        // Unique SKU index by shop, done with raw SQL in migrations

        // Relationships
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVariants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
