using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;


/// HDHiep - 10/04/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="UserRole"/> entity.
/// Defines the table name, primary key, property configurations, and indexes.
/// Implements the <see cref="IEntityTypeConfiguration{T}"/> interface.
/// Configures properties such as UserId, RoleId, AssignedAtUtc, and AssignedBy
/// with appropriate constraints and data types.
/// Sets maximum lengths and required constraints where applicable.
/// Also sets up composite primary key for UserId and RoleId.
/// </summary>
internal sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ur => ur.RoleId)
            .IsRequired();

        builder
            .Property(ur => ur.AssignedAtUtc)
            .IsRequired();

        builder.Property(ur => ur.AssignedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ur => ur.UserId);
        builder.HasIndex(ur => ur.RoleId);

    }
}
