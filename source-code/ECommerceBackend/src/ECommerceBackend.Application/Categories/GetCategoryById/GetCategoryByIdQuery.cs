using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Categories.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryResponse>;
