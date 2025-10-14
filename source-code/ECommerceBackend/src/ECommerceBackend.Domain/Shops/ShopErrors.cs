using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Shops;
public static class ShopErrors
{
    public static Error NotFound(Guid shopId) =>
        Error.NotFound("Shops.NotFound", $"The shop with the identifier {shopId} not found");
}
