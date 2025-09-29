using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Users.GetUserById;

public sealed record GetCategoryByIdQuery(Guid UserId) : IQuery<CategoryResponse>;
