using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Domain.Shops;

namespace ECommerceBackend.Application.Shops.GetAllShops;
public sealed record GetAllShopsQuery : IQuery<List<Shop>>;
