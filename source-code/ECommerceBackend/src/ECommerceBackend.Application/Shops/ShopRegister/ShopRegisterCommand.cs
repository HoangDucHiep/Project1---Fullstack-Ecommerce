using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Shops.ShopRegister;
public sealed record ShopRegisterCommand(
    string Name, string Description,
    string LogoUrl, string BannerUrl,
    Guid OwnerId
) : ICommand<Guid>;
