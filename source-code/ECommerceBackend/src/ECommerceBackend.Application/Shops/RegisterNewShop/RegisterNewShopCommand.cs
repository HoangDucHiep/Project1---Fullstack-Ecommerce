using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.RegisterNewShop;

public sealed record RegisterNewShopCommand(
    string Name,
    string Description,
    string LogoUrl,
    string BannerUrl,
    Guid OwnerId
) : ICommand<Guid>;
