using ECommerceBackend.Application.Abstracts.Messaging;

namespace ECommerceBackend.Application.Roles.CreateRole;
public record CreateRoleCommand(
    string Code,
    string Name,
    string? Description,
    IEnumerable<string> PermissionCodes
) : ICommand<Guid>;
