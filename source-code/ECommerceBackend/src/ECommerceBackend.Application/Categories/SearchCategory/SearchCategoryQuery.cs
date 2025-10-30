using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Categories;

namespace ECommerceBackend.Application.Categories.SearchCategory;

/// PBNMinh- 08/09/2025
public sealed record SearchCategoryQuery(string QueryText) : IQuery<List<CategoryTreeDto>>;

