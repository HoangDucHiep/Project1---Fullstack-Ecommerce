using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Users;
using ECommerceBackend.Infrastructure.Authentication;
using ECommerceBackend.Infrastructure.Caching;
using ECommerceBackend.Infrastructure.Clock;
using ECommerceBackend.Infrastructure.Data;
using ECommerceBackend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using StackExchange.Redis;

namespace ECommerceBackend.Infrastructure;

/// HDHiep - 09/24/2025
/// <summary>
/// Provides DI registrations for infrastructure services such as database, caching, and time providers.
/// </summary>
public static class InfrastructureConfiguration
{

    /// <summary>
    /// Registers infrastructure services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConnectionString">The PostgreSQL connection string.</param>
    /// <param name="redisConnectionString">The Redis connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {

        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        AddPersistence(services, configuration);

        return services;
    }


    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database")!; // PostgreSQL
        string redisConnectionString = configuration.GetConnectionString("Cache")!; // Redis

        //Console.WriteLine($"Database Connection String: {databaseConnectionString}");
        //Console.WriteLine($"Redis Connection String: {redisConnectionString}");

        // Register authentication services
        services.AddAuthenticationInternal();

        // Register NpgsqlDataSource and related services
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();

        services.TryAddSingleton(npgsqlDataSource);
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();


        // Add Redis caching with fallback to in-memory cache
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.TryAddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
            });
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        // Register CacheService
        services.TryAddSingleton<ICacheService, CacheService>();



        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString).UseSnakeCaseNamingConvention();
        });


        /// Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.TryAddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());


    }




}
