using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.IdentityId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(u => u.Email)
            .IsRequired(false)
            .HasMaxLength(255);
        builder.Property(u => u.Phone)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.Property(u => u.CreatedAtUtc)
            .IsRequired();
        builder.Property(u => u.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(u => u.IdentityId).IsUnique();

        // Unique indexes that ignore NULL values
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter($"\"{nameof(User.Email).ToLowerInvariant()}\" IS NOT NULL");

        builder.HasIndex(u => u.Phone)
            .IsUnique()
            .HasFilter($"\"{nameof(User.Phone).ToLowerInvariant()}\" IS NOT NULL");
    }
}
