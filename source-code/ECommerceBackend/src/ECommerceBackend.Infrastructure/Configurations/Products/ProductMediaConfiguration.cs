using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.ToTable("product_medias", "ecommerce-domain");

        builder.HasKey(pm => pm.Id);
        builder.Property(pm => pm.Id).HasColumnName("id");

        builder.Property(pm => pm.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(pm => pm.ProductVariantId)
            .HasColumnName("product_variant_id");

        builder.Property(pm => pm.MediaId)
            .HasColumnName("media_id")
            .IsRequired();

        builder.Property(pm => pm.IsCover)
            .HasColumnName("is_cover")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pm => pm.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Video: -1, Images: >= 0");

        builder.Property(pm => pm.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pm => pm.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        // Query filter for soft delete
        builder.HasQueryFilter(pm => !pm.IsDeleted);

        // Relationships
        builder.HasOne(pm => pm.Product)
            .WithMany(p => p.ProductMedias)
            .HasForeignKey(pm => pm.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.ProductVariant)
            .WithMany()
            .HasForeignKey(pm => pm.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.Media)
            .WithMany()
            .HasForeignKey(pm => pm.MediaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pm => pm.ProductId).HasDatabaseName("ix_product_medias_product_id");
        builder.HasIndex(pm => pm.ProductVariantId).HasDatabaseName("ix_product_medias_product_variant_id");
        builder.HasIndex(pm => pm.MediaId).HasDatabaseName("ix_product_medias_media_id");
        builder.HasIndex(pm => new { pm.ProductId, pm.SortOrder }).HasDatabaseName("ix_product_medias_product_sort_order");
        builder.HasIndex(pm => new { pm.ProductId, pm.IsCover }).HasDatabaseName("ix_product_medias_product_cover");
        builder.HasIndex(pm => new { pm.ProductVariantId, pm.IsCover }).HasDatabaseName("ix_product_medias_variant_cover");
    }
}
