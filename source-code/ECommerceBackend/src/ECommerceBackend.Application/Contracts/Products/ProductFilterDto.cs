namespace ECommerceBackend.Application.Contracts.Products;

public record ProductFilterDto(
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? HasDiscount = null,
    Guid? ShopId = null,
    ProductSortBy SortBy = ProductSortBy.Relevance,
    int Page = 1,
    int PageSize = 20
);

public enum ProductSortBy
{
    Relevance = 0,
    Newest = 1,
    PriceLowToHigh = 2,
    PriceHighToLow = 3
}
