using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
