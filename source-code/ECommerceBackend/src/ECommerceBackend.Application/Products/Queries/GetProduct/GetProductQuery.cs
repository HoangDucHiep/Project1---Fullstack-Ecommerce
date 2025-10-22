using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProduct;

public record GetProductQuery(
    Guid ProductId
) : IQuery<ProductDetailDto>;
