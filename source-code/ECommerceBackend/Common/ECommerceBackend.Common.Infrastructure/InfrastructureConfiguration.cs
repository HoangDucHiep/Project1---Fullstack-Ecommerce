using ECommerceBackend.Common.Application.Caching;
using ECommerceBackend.Common.Application.Clock;
using ECommerceBackend.Common.Application.Data;
using ECommerceBackend.Common.Infrastructure.Caching;
using ECommerceBackend.Common.Infrastructure.Clock;
using ECommerceBackend.Common.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using StackExchange.Redis;

namespace ECommerceBackend.Common.Infrastructure;
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
        string databaseConnectionString,
        string redisConnectionString
    )
    {
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();

        services.TryAddSingleton(npgsqlDataSource);
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        // redis
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

        services.TryAddSingleton<ICacheService, CacheService>();

        return services;
    }


}
