using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Shops;
using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.GetShopByUserId;

internal sealed class GetShopByUserIdQueryHandler : IQueryHandler<GetShopByUserIdQuery, ShopDto?>
{
    private readonly IShopRepository _shopRepository;

    public GetShopByUserIdQueryHandler(IShopRepository shopRepository)
    {
        _shopRepository = shopRepository;
    }

    public async Task<Result<ShopDto?>> Handle(GetShopByUserIdQuery request, CancellationToken cancellationToken)
    {
        Shop? shop = await _shopRepository.GetByOwnerIdAsync(request.UserId, cancellationToken);

        if (shop == null)
        {
            // User chưa có shop, trả về null thay vì error
            return Result.Success<ShopDto?>(null);
        }

        var shopDto = new ShopDto(
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

        return Result.Success<ShopDto?>(shopDto);
    }
}


