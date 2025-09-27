using ECommerceBackend.Domain.Abstracts;
using ECommerceBackend.Domain.Utils;

namespace ECommerceBackend.Domain.Shops;


public enum ShopStatus
{
    PENDING,
    ACTIVE,
    INACTIVE,
    BLOCKED,
    DELETED
}

public class Shop : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string LogoUrl { get; private set; }
    public string BannerUrl { get; private set; }
    public ShopStatus Status { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    private Shop()
    {
        // For ORM
    }
    public static Shop Create(string name, string description, string logoUrl, string bannerUrl, Guid ownerId)
    {
        var shop = new Shop
        {
            Id = IdGenerator.GenerateId(),
            Name = name,
            Description = description,
            LogoUrl = logoUrl,
            BannerUrl = bannerUrl,
            OwnerId = ownerId,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            Status = ShopStatus.PENDING
        };
        // Raise domain event if needed
        return shop;
    }

    public bool Approve()
    {
        if (Status != ShopStatus.PENDING)
        {
            return false;
        }

        Status = ShopStatus.ACTIVE;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        // Raise domain event if needed
        return true;
    }

    public void Update(string name, string description, string logoUrl, string bannerUrl)
    {
        Name = name;
        Description = description;
        LogoUrl = logoUrl;
        BannerUrl = bannerUrl;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
        // Raise domain event if needed
    }
}
