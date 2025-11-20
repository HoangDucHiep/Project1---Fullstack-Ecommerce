namespace ECommerceBackend.Api.Controllers.Shops;

/// <summary>
/// Request DTO for registering a new shop
/// </summary>
public record RegisterNewShopRequest(
    string Name,
    string Description,
    string LogoUrl,
    string BannerUrl
);

