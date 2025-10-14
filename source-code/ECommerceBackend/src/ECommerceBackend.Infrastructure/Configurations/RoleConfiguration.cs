using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;


/// HDHiep - 10/04/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="Role"/> entity.
/// Defines the table name, primary key, property configurations, and indexes.
/// Implements the <see cref="IEntityTypeConfiguration{T}"/> interface.
/// Configures properties such as Name, Code, IsSystemRole, CreatedAtUtc, and UpdatedAtUtc
/// with appropriate constraints and data types.
/// Sets maximum lengths and required constraints where applicable.
/// </summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(r => r.Code)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(r => r.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(r => r.CreatedAtUtc)
            .IsRequired();
        builder.Property(r => r.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(r => r.Code).IsUnique();
    }
}
