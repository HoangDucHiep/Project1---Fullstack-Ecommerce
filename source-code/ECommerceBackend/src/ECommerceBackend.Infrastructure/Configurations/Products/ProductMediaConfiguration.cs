using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;
internal sealed class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.ToTable("product_medias");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.ProductId)
            .IsRequired();

        builder.Property(pm => pm.ProductVariantId)
            .IsRequired(false);

        builder.Property(pm => pm.MediaUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pm => pm.MediaType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(pm => pm.SortOrder)
            .IsRequired();

        builder.Property(pm => pm.CreatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(pm => pm.ProductId);
        builder.HasIndex(pm => pm.ProductVariantId);
        builder.HasIndex(pm => new { pm.ProductId, pm.SortOrder });

        // Relationships
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(pm => pm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProductVariant>()
            .WithMany()
            .HasForeignKey(pm => pm.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
