using Microsoft.Extensions.DependencyInjection;

namespace ECommerceBackend.Infrastructure.Authentication;

/// HDHiep - 09/24/2025
/// <summary>
/// Provides extension methods for registering authentication services.
/// </summary>
internal static class AuthenticationExtensions
{
    /// <summary>
    /// Registers authentication services.
    /// </summary>
    internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        // Add authorization services
        services.AddAuthorization();

        // Add authentication services 
        services.AddAuthentication().AddJwtBearer();

        // Add HttpContextAccessor for accessing HTTP context in services
        services.AddHttpContextAccessor();

        // Configure JwtBearerOptions using JwtBearerConfigurationOptions
        services.ConfigureOptions<JwtBearerConfigurationOptions>();

        // Add authentication services here
        return services;
    }
}

