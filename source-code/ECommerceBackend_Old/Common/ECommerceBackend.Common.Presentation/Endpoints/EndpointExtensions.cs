using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ECommerceBackend.Common.Presentation.Endpoints;

/// <summary>
/// Provides extension methods to register and map minimal API endpoints.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Scans the provided assemblies for implementations of <see cref="IEndpoint"/>
    /// and registers them with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <param name="assemblies">Assemblies to scan for <see cref="IEndpoint"/> implementations.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, params Assembly[] assemblies)
    {
        ServiceDescriptor[] serviceDescriptors = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    /// Maps all registered <see cref="IEndpoint"/> implementations to the given route builder.
    /// </summary>
    /// <param name="app">The application instance used to resolve registered endpoints.</param>
    /// <param name="routeGroupBuilder">Optional route group to map endpoints under; if null, maps on the app root.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> for chaining.</returns>
    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoints(builder);
        }

        return app;
    }
}
