using Microsoft.Extensions.DependencyInjection;

namespace ECommerceBackend.Common.Infrastructure.Authentication;

/// <summary>
/// HoangDucHiep - 08/17/2025
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
