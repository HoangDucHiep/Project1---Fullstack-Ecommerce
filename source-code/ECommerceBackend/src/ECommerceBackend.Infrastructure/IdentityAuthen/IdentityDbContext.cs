using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Infrastructure.IdentityAuthen;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Infrastructure.Identity;

public class IdentityDbContext : IdentityDbContext<ApplicationIdentityUser>, IIdentityUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Custom
        builder.HasDefaultSchema(Schemas.Identity);

        // Configure table
        builder.Entity<ApplicationIdentityUser>(entity =>
        {
            entity.ToTable(name: "Users");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Email).HasMaxLength(256);
            entity.Property(t => t.NormalizedEmail).HasMaxLength(256);

            entity.Property(t => t.PhoneNumber).HasMaxLength(15);

            entity.HasIndex(t => t.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique(false);

            entity.HasIndex(t => t.PhoneNumber).HasDatabaseName("PhoneNumberIndex").IsUnique(false);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Token).IsRequired();
            entity.Property(t => t.JwtId).IsRequired();
            entity.Property(t => t.CreatedAtUtc).IsRequired();
            entity.Property(t => t.ExpiresAtUtc).IsRequired();
            entity.Property(t => t.IsUsed).IsRequired();
            entity.Property(t => t.IsRevoked).IsRequired();
            entity.HasOne(t => t.IdentityUser)
                  .WithMany()
                  .HasForeignKey(t => t.IdentityUserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Identity tables with custom names
        builder.Entity<IdentityRole>(entity => entity.ToTable("Roles"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("RoleClaims"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens"));
    }


    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(new Error("ConcurrencyException", "Concurrency exception occurred in ApplicationDbContext", ErrorType.Conflict), ex);
        }
    }
}
