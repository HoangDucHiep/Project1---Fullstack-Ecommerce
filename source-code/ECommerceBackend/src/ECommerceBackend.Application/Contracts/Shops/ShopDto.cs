using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Contracts.Shops;

/// <summary>
/// 
/// </summary>
internal sealed record ShopDto // : ILinksResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string LogoUrl { get; init; }
    public string BannerUrl { get; init; }
    public ShopStatus Status { get; init; }
    public Guid OwnerId { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset UpdatedAtUtc { get; init; }
}
