using ECommerceBackend.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;

/// HDHiep - 09/27/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="Category"/> entity.
/// Defines the table name, primary key, property configurations, and indexes.
/// Implements the <see cref="IEntityTypeConfiguration{T}"/> interface.
/// Configures properties such as Name, IconUrl, Status, ParentId, Lft, Rgt, Depth, CreatedAtUtc, and UpdatedAtUtc
/// with appropriate constraints and data types.
/// Sets maximum lengths and required constraints where applicable.
/// </summary>
internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);
        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(c => c.ParentId);
        builder.Property(c => c.Lft)
            .IsRequired();
        builder.Property(c => c.Rgt)
            .IsRequired();
        builder.Property(c => c.Depth)
            .IsRequired();
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();
        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();
        builder.HasIndex(c => c.Name).IsUnique(false);

        // for hierarchical queries, consider indexing Lft and Rgt
        builder.HasIndex(c => new { c.Lft, c.Rgt });
        builder.HasIndex(c => new { c.ParentId });
    }
}
