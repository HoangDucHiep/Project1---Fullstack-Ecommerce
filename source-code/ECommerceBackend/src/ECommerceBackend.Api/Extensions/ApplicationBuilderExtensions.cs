using ECommerceBackend.Api.Middlewares;
using ECommerceBackend.Infrastructure;
using ECommerceBackend.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceBackend.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();

        using IdentityDbContext idDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        idDbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandler>();
    }
}
