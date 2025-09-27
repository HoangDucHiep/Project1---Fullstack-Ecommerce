using ECommerceBackend.Domain.Shops;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;
internal sealed class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> builder)
    {
        builder.ToTable("shops");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(s => s.Description)
            .HasMaxLength(1000);
        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);
        builder.Property(s => s.BannerUrl)
            .HasMaxLength(500);
        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(s => s.OwnerId)
            .IsRequired();
        builder.Property(s => s.CreatedAtUtc)
            .IsRequired();
        builder.Property(s => s.UpdatedAtUtc)
            .IsRequired();
        builder.HasIndex(s => s.OwnerId).IsUnique();

    }
}
