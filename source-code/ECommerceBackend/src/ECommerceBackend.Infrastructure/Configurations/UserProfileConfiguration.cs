using ECommerceBackend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerceBackend.Infrastructure.Configurations;


/// HDHiep - 10/04/2025
/// <summary>
/// Entity Framework Core configuration for the <see cref="UserProfile"/> entity.
/// Defines the table name, primary key, property configurations, and indexes.
/// Implements the <see cref="IEntityTypeConfiguration{T}"/> interface.
/// Configures properties such as UserId, Avatar_Url, Bio, IdCardFullName, IdCardNumber, and IdCardFullAddress
/// with appropriate constraints and data types.
/// Sets maximum lengths and required constraints where applicable.
/// Also sets up a unique index for UserId.
/// </summary>
internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.HasKey(up => up.Id);
        builder.Property(up => up.UserId)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(up => up.Avatar_Url)
            .HasMaxLength(500);
        builder.Property(up => up.Bio)
            .HasMaxLength(1000);
        builder.Property(up => up.IdCardFullName)
            .HasMaxLength(200);
        builder.Property(up => up.IdCardNumber)
            .HasMaxLength(50);
        builder.Property(up => up.IdCardFullAddress)
            .HasMaxLength(500);
        builder.HasIndex(up => up.UserId).IsUnique();
        // Relationships
        builder.HasOne<User>()
               .WithOne()
               .HasForeignKey<UserProfile>(up => up.UserId)
               .OnDelete(DeleteBehavior.Cascade);
        // Indexes
        builder.HasIndex(up => up.UserId).IsUnique();
    }
}
