using ECommerceBackend.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations.Products;


/// <summary>
/// Entity Framework Core configuration for the <see cref="ProductOptionType"/> entity.
/// </summary>
internal sealed class ProductOptionTypeConfiguration : IEntityTypeConfiguration<ProductOptionType>
{
    public void Configure(EntityTypeBuilder<ProductOptionType> builder)
    {
        builder.ToTable("product_option_types");

        builder.HasKey(pot => pot.Id);

        builder.Property(pot => pot.ProductId)
            .IsRequired();

        builder.Property(pot => pot.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pot => pot.CreatedAtUtc)
            .IsRequired();

        builder.Property(pot => pot.UpdatedAtUtc)
            .IsRequired();

        // Indexes
        builder.HasIndex(pot => pot.ProductId);
        builder.HasIndex(pot => new { pot.ProductId, pot.Name }).IsUnique();

        // Relationships
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(pot => pot.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
