using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;

public sealed class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; }
    public string? AssignedBy { get; private set; } // IdentityId of who assigned this role

    private UserRole(Guid userId, Guid roleId, string? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAtUtc = DateTimeOffset.UtcNow;
        AssignedBy = assignedBy;
    }

    public static UserRole Create(Guid userId, Guid roleId, string? assignedBy = null)
    {
        return new UserRole(userId, roleId, assignedBy);
    }
}
