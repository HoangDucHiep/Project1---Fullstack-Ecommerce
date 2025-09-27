using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Api.Controllers.Shops;

public sealed record ShopRegiesterRequest(
    string Name, string Description,
    string LogoUrl, string BannerUrl,
    ShopStatus Status, Guid OwnerId
);
