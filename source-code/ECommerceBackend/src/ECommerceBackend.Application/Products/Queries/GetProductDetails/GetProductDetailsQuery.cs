using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Products;

namespace ECommerceBackend.Application.Products.Queries.GetProductDetails;

public sealed record GetProductDetailsQuery(Guid ProductId) : IQuery<ProductDetailDto>;
