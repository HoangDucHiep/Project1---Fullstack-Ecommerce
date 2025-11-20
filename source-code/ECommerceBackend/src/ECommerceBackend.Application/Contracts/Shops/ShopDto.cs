using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Contracts.Shops;

public record ShopDto(
    Guid Id,
    string Name,
    string Description,
    string LogoUrl,
    string BannerUrl,
    ShopStatus Status,
    Guid OwnerId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc
);
