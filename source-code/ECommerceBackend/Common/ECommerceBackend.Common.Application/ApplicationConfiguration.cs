using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceBackend.Common.Application;
public static class ApplicationConfiguration
{
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
