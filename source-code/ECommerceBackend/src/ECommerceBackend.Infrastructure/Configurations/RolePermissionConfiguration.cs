using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;


/// HDHiep - 10/04/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="RolePermission"/> entity.
/// Defines the table mapping, keys, properties, relationships, and indexes.
/// This configuration ensures that the <see cref="RolePermission"/> entity is properly mapped to the "role_permissions" table in the database.
/// It establishes a composite primary key consisting of RoleId and PermissionId, and sets up the necessary foreign key relationships to the Role and Permission entities.
/// It also configures the properties with appropriate constraints and indexes to optimize query performance.
/// This configuration is essential for maintaining data integrity and enforcing the relationships between roles and permissions within the application's user management system.
/// </summary>
internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.RoleId)
            .IsRequired();
        builder.Property(rp => rp.PermissionId)
            .IsRequired();
        builder.Property(rp => rp.PermissionCode)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(rp => rp.GrantedAtUtc)
            .IsRequired();


        // Relationships
        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rp => rp.RoleId);
        builder.HasIndex(rp => rp.PermissionId);
        builder.HasIndex(rp => rp.PermissionCode);
    }
}
