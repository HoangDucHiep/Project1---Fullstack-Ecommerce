using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Contracts.Shops;
internal static class ShopMappings
{
    public static ShopDto ToShopDto(this Shop shop)
    {
        return new ShopDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            LogoUrl = shop.LogoUrl,
            BannerUrl = shop.BannerUrl,
            Status = shop.Status,
            OwnerId = shop.OwnerId,
            CreatedAtUtc = shop.CreatedAtUtc,
            UpdatedAtUtc = shop.UpdatedAtUtc
        };
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
