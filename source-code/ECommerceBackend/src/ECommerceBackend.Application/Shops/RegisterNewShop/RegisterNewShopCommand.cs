using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.RegisterNewShop;
public sealed record RegisterNewShopCommand
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string LogoUrl { get; private set; }
    public string BannerUrl { get; private set; }
    public ShopStatus Status { get; private set; }
    public Guid OwnerId { get; private set; }
}
