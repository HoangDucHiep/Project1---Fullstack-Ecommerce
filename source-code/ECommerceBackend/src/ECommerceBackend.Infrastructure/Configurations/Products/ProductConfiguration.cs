using ECommerceBackend.Domain.Products;
using ECommerceBackend.Domain.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


/// HDHiep - 10/05/2025
/// <summary>
/// Configuration for the Product entity.
/// </summary>
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ShopId)
            .IsRequired();
        builder.Property(p => p.CategoryId)
            .IsRequired();
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(p => p.Description)
            .IsRequired()
            .HasColumnType("text");
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();
        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.ShopId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Description);

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId) // Explicitly use the existing CategoryId property
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Shop>()
            .WithMany()
            .HasForeignKey(p => p.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
