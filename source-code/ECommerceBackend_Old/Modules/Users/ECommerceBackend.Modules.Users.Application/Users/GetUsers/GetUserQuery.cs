using ECommerceBackend.Common.Application.Messaging;

namespace ECommerceBackend.Modules.Users.Application.Users.GetUsers;
public sealed record GetUserQuery(Guid UserId) : IQuery<UserResponse>;
