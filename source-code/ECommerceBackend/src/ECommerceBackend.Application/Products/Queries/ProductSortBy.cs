namespace ECommerceBackend.Application.Products.Queries;

public enum ProductSortBy
{
    Relevance,    // Default when Q is provided
    Newest,       // CreatedAtUtc DESC
    PriceAsc,     // Min price ASC
    PriceDesc     // Min price DESC
}
