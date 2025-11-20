using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Shops;

namespace ECommerceBackend.Application.Shops.GetShopByUserId;

public sealed record GetShopByUserIdQuery(
    Guid UserId
) : IQuery<ShopDto?>;


