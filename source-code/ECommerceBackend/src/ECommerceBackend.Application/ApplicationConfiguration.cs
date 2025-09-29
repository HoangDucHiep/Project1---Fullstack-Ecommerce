using ECommerceBackend.Application.Abstracts.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceBackend.Application;

/// HDHiep - 09/24/2025
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
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(ApplicationConfiguration).Assembly);

            // Exception handling behavior
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));

            // Logging behavior
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));

            // Validation behavior
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        // add validation.
        services.AddValidatorsFromAssembly(typeof(ApplicationConfiguration).Assembly, includeInternalTypes: true);

        return services;
    }
}
