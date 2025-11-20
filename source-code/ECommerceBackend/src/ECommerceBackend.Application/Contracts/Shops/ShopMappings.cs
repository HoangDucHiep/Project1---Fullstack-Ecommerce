using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Contracts.Shops;
internal static class ShopMappings
{
    public static ShopDto ToShopDto(this Shop shop)
    {
        return new ShopDto(
            shop.Id,
            shop.Name,
            shop.Description,
            shop.LogoUrl,
            shop.BannerUrl,
            shop.Status,
            shop.OwnerId,
            shop.CreatedAtUtc,
            shop.UpdatedAtUtc
        );
    }

    public static Shop ToShopEntity(this ShopDto shopDto)
    {
        return Shop.Create(
            shopDto.Name,
            shopDto.Description,
            shopDto.LogoUrl,
            shopDto.BannerUrl,
            shopDto.OwnerId
        );
    }
}
