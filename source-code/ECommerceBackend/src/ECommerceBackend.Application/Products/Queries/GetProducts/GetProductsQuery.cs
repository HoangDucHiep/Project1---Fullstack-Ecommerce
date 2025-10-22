using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Commons;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    ProductFilterDto Filter
) : IQuery<PaginationResult<ProductListItemDto>>;
