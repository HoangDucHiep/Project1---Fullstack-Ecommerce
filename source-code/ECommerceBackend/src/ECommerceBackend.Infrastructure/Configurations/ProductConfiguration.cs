using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Products;
using ECommerceBackend.Domain.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;

/// HDHiep - 10/05/2025
/// <summary>
/// Configuration for the Product entity.
/// </summary>
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ShopId)
            .IsRequired();
        builder.Property(p => p.CategoryId)
            .IsRequired();
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(1000);
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(p => p.Status)
            .IsRequired();
        builder.Property(p => p.Medias)
            .IsRequired()
            .HasMaxLength(2000);
        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();
        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired();
        // Indexes
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.ShopId);
        builder.HasIndex(p => p.CategoryId);

        // Relationships
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Shop>()
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
