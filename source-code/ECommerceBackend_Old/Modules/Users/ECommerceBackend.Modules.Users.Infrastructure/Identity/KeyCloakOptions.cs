namespace ECommerceBackend.Modules.Users.Infrastructure.Identity;

/// HoangDucHiep - 08/2025
/// <summary>
/// Configuration options for KeyCloak identity provider.
/// </summary>
/// <remarks>
/// This class holds the necessary configuration settings required to interact with KeyCloak,
/// including URLs and client credentials.
/// </remarks>
internal sealed class KeyCloakOptions
{
    public string AdminUrl { get; set; }
    public string TokenUrl { get; set; }
    public string ConfidentialClientId { get; set; }
    public string ConfidentialClientSecret { get; set; }
    public string PublicClientId { get; set; }
}
