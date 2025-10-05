using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.Users;


/// HDHiep - 10/04/2025
/// <summary>
/// Represents the relationship between User and Role with additional metadata
/// </summary>
public class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; }
    public Guid? AssignedBy { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    private UserRole() { } // For EF Core

    public static UserRole Create(Guid userId, Guid roleId, Guid? assignedBy)
    {
        return new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAtUtc = DateTimeOffset.UtcNow,
            AssignedBy = assignedBy
        };
    }

    public static UserRole Create(User user, Role role, Guid? assignedBy)
    {
        return new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role,
            AssignedAtUtc = DateTimeOffset.UtcNow,
            AssignedBy = assignedBy
        };
    }
}
