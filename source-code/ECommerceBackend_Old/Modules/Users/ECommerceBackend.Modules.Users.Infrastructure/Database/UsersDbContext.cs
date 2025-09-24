using ECommerceBackend.Modules.Users.Application.Abstractions.Data;
using ECommerceBackend.Modules.Users.Domain.Users;
using ECommerceBackend.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Modules.Users.Infrastructure.Database;
public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options), IUnitOfWork
{
    internal DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Users);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
