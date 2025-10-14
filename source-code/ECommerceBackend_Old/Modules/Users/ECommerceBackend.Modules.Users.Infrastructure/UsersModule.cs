using ECommerceBackend.Common.Presentation.Endpoints;
using ECommerceBackend.Modules.Users.Application.Abstractions.Data;
using ECommerceBackend.Modules.Users.Application.Identity;
using ECommerceBackend.Modules.Users.Domain.Users;
using ECommerceBackend.Modules.Users.Infrastructure.Database;
using ECommerceBackend.Modules.Users.Infrastructure.Identity;
using ECommerceBackend.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Modules.Users.Infrastructure;

/// HoangDucHiep - 08/2025
/// <summary>
/// Extension methods for configuring the Users module.
/// This class provides methods to add and configure services related to the Users module,
/// including database context, identity provider, and HTTP clients.
/// It encapsulates the setup required to integrate the Users module into the application.
/// It is designed to be used in the application's startup configuration.
/// </summary>
public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Keycloak configuration
        services.Configure<KeyCloakOptions>(configuration.GetSection("Users:KeyCloak"));

        services.AddTransient<KeyCloakAuthDelegatingHandler>();

        // Register HttpClient for KeyCloakClient with authentication handler
        services.AddHttpClient<KeyCloakClient>((serviceProvider, httpClient) =>
        {
            KeyCloakOptions keyCloakOptions = serviceProvider
                .GetRequiredService<IOptions<KeyCloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keyCloakOptions.AdminUrl);
        })
            .AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>();

        services.AddTransient<IIdentityProviderService, IdentityProviderService>();


        services.AddDbContext<UsersDbContext>((sp, options) =>
           options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users
                ))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
    }
}
