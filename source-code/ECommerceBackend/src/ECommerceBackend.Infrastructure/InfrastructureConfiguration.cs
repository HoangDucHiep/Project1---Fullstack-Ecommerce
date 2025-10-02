using System.Text;
using ECommerceBackend.Application.Abstracts.Authentication;
using ECommerceBackend.Application.Abstracts.Caching;
using ECommerceBackend.Application.Abstracts.Clock;
using ECommerceBackend.Application.Abstracts.Data;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Addresses;
using ECommerceBackend.Domain.Categories;
using ECommerceBackend.Domain.Shops;
using ECommerceBackend.Domain.Users;
using ECommerceBackend.Infrastructure.Authentication;
using ECommerceBackend.Infrastructure.Caching;
using ECommerceBackend.Infrastructure.Clock;
using ECommerceBackend.Infrastructure.Data;
using ECommerceBackend.Infrastructure.Identity;
using ECommerceBackend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
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
        AddAuthentication(services, configuration);

        return services;
    }


    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database")!; // PostgreSQL
        string redisConnectionString = configuration.GetConnectionString("Cache")!; // Redis

        //Console.WriteLine($"Database Connection String: {databaseConnectionString}");
        //Console.WriteLine($"Redis Connection String: {redisConnectionString}");


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


        // Application Domain DBContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString, options =>
            {
                options.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Application);
            }).UseSnakeCaseNamingConvention();
        });

        // Application Identity DbContext
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString, options =>
            {
                options.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Identity);
            }).UseSnakeCaseNamingConvention();
        });


        /// Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();

        services.TryAddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());


    }


    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = false; // We handle this manually
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        // Add JWT Authentication
        IConfigurationSection jwtSettings = configuration.GetSection("Jwt");
        string? secretKey = jwtSettings["SecretKey"];


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Add HttpContextAccessor
        services.AddHttpContextAccessor();
        // Register Authentication services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

}
