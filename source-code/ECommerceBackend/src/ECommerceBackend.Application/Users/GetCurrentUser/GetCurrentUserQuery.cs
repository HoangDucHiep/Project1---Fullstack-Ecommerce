using ECommerceBackend.Application.Abstracts.Messaging;
using ECommerceBackend.Application.Contracts.Users;

namespace ECommerceBackend.Application.Users.GetCurrentUser;

/// <summary>
/// Query to get current authenticated user information
/// </summary>
public sealed record GetCurrentUserQuery : IQuery<UserDto>;
