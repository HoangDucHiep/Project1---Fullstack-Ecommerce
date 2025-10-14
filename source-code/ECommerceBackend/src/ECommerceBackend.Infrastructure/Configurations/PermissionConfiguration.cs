using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;


/// HDHiep - 10/04/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="Permission"/> entity.
/// Defines the table name, primary key, property configurations, and indexes.
/// Implements the <see cref="IEntityTypeConfiguration{T}"/> interface.
/// Configures properties such as Name, Code, CreatedAtUtc, and UpdatedAtUtc
/// with appropriate constraints and data types.
/// Sets maximum lengths and required constraints where applicable.
/// </summary>
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();
        builder.Property(p => p.UpdatedAtUtc)
            .IsRequired();
        builder.HasIndex(p => p.Code).IsUnique();
    }
}
