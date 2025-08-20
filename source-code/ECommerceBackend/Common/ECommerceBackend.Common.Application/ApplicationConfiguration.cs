using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceBackend.Common.Application;
/// <summary>
/// Provides DI registrations for application-layer services such as MediatR, validation, and behaviors.
/// </summary>
public static class ApplicationConfiguration
{
    /// <summary>
    /// Registers application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="modulesAssemblies">Assemblies that contain MediatR handlers and behaviors.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly[] modulesAssemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(modulesAssemblies);

            // Add behaviors heres
        });

        // add validation.

        return services;
    }
}
