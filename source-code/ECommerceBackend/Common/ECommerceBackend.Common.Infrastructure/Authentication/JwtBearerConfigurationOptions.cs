using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ECommerceBackend.Common.Infrastructure.Authentication;

/// <summary>
/// HoangDucHiep - 08/17/2025
/// Configures JwtBearerOptions using settings from the "Authentication" section of the configuration.
/// </summary>
/// <param name="configuration">The configuration.</param>
internal sealed class JwtBearerConfigurationOptions(IConfiguration configuration) : IConfigureNamedOptions<JwtBearerOptions>
{

    private const string ConfigurationSectionName = "Authentication";

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}
