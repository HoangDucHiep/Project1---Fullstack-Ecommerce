using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.RBAC;

public enum RoleType
{
    System,  // Admin, Seller, Buyer
    Custom   // Created by Admin
}

public sealed class Role : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public RoleType Type { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role(string code, string name, string? description, RoleType type)
    {
        Code = code;
        Name = name;
        Description = description;
        Type = type;
        IsActive = true;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public static Role CreateSystemRole(string code, string name, string? description = null)
    {
        return new Role(code, name, description, RoleType.System);
    }

    public static Role CreateCustomRole(string code, string name, string? description = null)
    {
        return new Role(code, name, description, RoleType.Custom);
    }

    public void UpdateInfo(string name, string? description = null)
    {
        if (Type == RoleType.System)
        {
            throw new InvalidOperationException("Cannot update system roles");
        }

        Name = name;
        Description = description;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void AddPermission(string permissionCode)
    {
        if (_rolePermissions.Any(rp => rp.PermissionCode == permissionCode))
        {
            return; // Permission already exists
        }

        _rolePermissions.Add(RolePermission.Create(Id, permissionCode));
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void RemovePermission(string permissionCode)
    {
        RolePermission? rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionCode == permissionCode);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void SetPermissions(IEnumerable<string> permissionCodes)
    {
        _rolePermissions.Clear();
        foreach (string permissionCode in permissionCodes)
        {
            _rolePermissions.Add(RolePermission.Create(Id, permissionCode));
        }
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        if (Type == RoleType.System)
        {
            throw new InvalidOperationException("Cannot deactivate system roles");
        }

        IsActive = false;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public bool HasPermission(string permissionCode)
    {
        return _rolePermissions.Any(rp => rp.PermissionCode == permissionCode);
    }

    // System Roles - Predefined
    public static class SystemRoles
    {
        public static readonly Role Admin = CreateSystemRole("admin", "Administrator", "Full system access");
        public static readonly Role Seller = CreateSystemRole("seller", "Seller", "Can manage own shop and products");
        public static readonly Role Buyer = CreateSystemRole("buyer", "Buyer", "Can browse and purchase products");

        public static IReadOnlyList<Role> GetAll()
        {
            // Configure permissions for each system role
            Role admin = CreateSystemRole("admin", "Administrator", "Full system access");
            admin.SetPermissions(
            [
                "users.view", "users.create", "users.update", "users.delete",
                "roles.view", "roles.create", "roles.update", "roles.delete", "roles.assign",
                "products.view", "products.create", "products.update", "products.delete", "products.inventory",
                "orders.view", "orders.update_status", "orders.cancel", "orders.refund",
                "shops.view", "shops.create", "shops.update", "shops.delete",
                "categories.view", "categories.create", "categories.update", "categories.delete",
                "system.logs", "system.settings",
                "reports.sales", "reports.users", "reports.system"
            ]);

            var seller = CreateSystemRole("seller", "Seller", "Can manage own shop and products");
            seller.SetPermissions(
            [
                "products.view", "products.create", "products.update", "products.delete", "products.inventory",
                "orders.view", "orders.update_status",
                "shops.view", "shops.manage_products",
                "categories.view",
                "reports.sales"
            ]);

            var buyer = CreateSystemRole("buyer", "Buyer", "Can browse and purchase products");
            buyer.SetPermissions(
            [
                "products.view",
                "shops.view",
                "categories.view"
            ]);

            return new List<Role> { admin, seller, buyer };
        }
    }
}
