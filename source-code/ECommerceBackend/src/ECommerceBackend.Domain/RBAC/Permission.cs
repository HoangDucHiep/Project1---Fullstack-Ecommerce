using ECommerceBackend.Domain.Abstracts;

namespace ECommerceBackend.Domain.RBAC;


public sealed class Permission : Entity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }

    private Permission(string code, string name, string description, string category)
    {
        Code = code;
        Name = name;
        Description = description;
        Category = category;
    }

    public static Permission Create(string code, string name, string description, string category)
    {
        return new Permission(code, name, description, category);
    }

    // System Permissions - Predefined
    public static class SystemPermissions
    {
        // User Management
        public static readonly Permission ViewUsers = Create("users.view", "View Users", "Can view user list and details", "User Management");
        public static readonly Permission CreateUsers = Create("users.create", "Create Users", "Can create new users", "User Management");
        public static readonly Permission UpdateUsers = Create("users.update", "Update Users", "Can update user information", "User Management");
        public static readonly Permission DeleteUsers = Create("users.delete", "Delete Users", "Can delete users", "User Management");

        // Role Management
        public static readonly Permission ViewRoles = Create("roles.view", "View Roles", "Can view roles and permissions", "Role Management");
        public static readonly Permission CreateRoles = Create("roles.create", "Create Roles", "Can create custom roles", "Role Management");
        public static readonly Permission UpdateRoles = Create("roles.update", "Update Roles", "Can update role permissions", "Role Management");
        public static readonly Permission DeleteRoles = Create("roles.delete", "Delete Roles", "Can delete custom roles", "Role Management");
        public static readonly Permission AssignRoles = Create("roles.assign", "Assign Roles", "Can assign roles to users", "Role Management");

        // Product Management
        public static readonly Permission ViewProducts = Create("products.view", "View Products", "Can view product list and details", "Product Management");
        public static readonly Permission CreateProducts = Create("products.create", "Create Products", "Can create new products", "Product Management");
        public static readonly Permission UpdateProducts = Create("products.update", "Update Products", "Can update product information", "Product Management");
        public static readonly Permission DeleteProducts = Create("products.delete", "Delete Products", "Can delete products", "Product Management");
        public static readonly Permission ManageProductInventory = Create("products.inventory", "Manage Inventory", "Can manage product inventory", "Product Management");

        // Order Management
        public static readonly Permission ViewOrders = Create("orders.view", "View Orders", "Can view order list and details", "Order Management");
        public static readonly Permission UpdateOrderStatus = Create("orders.update_status", "Update Order Status", "Can update order status", "Order Management");
        public static readonly Permission CancelOrders = Create("orders.cancel", "Cancel Orders", "Can cancel orders", "Order Management");
        public static readonly Permission ProcessRefunds = Create("orders.refund", "Process Refunds", "Can process order refunds", "Order Management");

        // Shop Management
        public static readonly Permission ViewShops = Create("shops.view", "View Shops", "Can view shop list and details", "Shop Management");
        public static readonly Permission CreateShops = Create("shops.create", "Create Shops", "Can create new shops", "Shop Management");
        public static readonly Permission UpdateShops = Create("shops.update", "Update Shops", "Can update shop information", "Shop Management");
        public static readonly Permission DeleteShops = Create("shops.delete", "Delete Shops", "Can delete shops", "Shop Management");
        public static readonly Permission ManageShopProducts = Create("shops.manage_products", "Manage Shop Products", "Can manage products in own shop", "Shop Management");

        // Category Management
        public static readonly Permission ViewCategories = Create("categories.view", "View Categories", "Can view category list", "Category Management");
        public static readonly Permission CreateCategories = Create("categories.create", "Create Categories", "Can create new categories", "Category Management");
        public static readonly Permission UpdateCategories = Create("categories.update", "Update Categories", "Can update categories", "Category Management");
        public static readonly Permission DeleteCategories = Create("categories.delete", "Delete Categories", "Can delete categories", "Category Management");

        // System Administration
        public static readonly Permission ViewSystemLogs = Create("system.logs", "View System Logs", "Can view system logs", "System Administration");
        public static readonly Permission ManageSystemSettings = Create("system.settings", "Manage System Settings", "Can manage system settings", "System Administration");

        // Reports
        public static readonly Permission ViewSalesReports = Create("reports.sales", "View Sales Reports", "Can view sales reports", "Reports");
        public static readonly Permission ViewUserReports = Create("reports.users", "View User Reports", "Can view user reports", "Reports");
        public static readonly Permission ViewSystemReports = Create("reports.system", "View System Reports", "Can view system reports", "Reports");

        public static IReadOnlyList<Permission> GetAll()
        {
            return new List<Permission>
            {
                ViewUsers, CreateUsers, UpdateUsers, DeleteUsers,
                ViewRoles, CreateRoles, UpdateRoles, DeleteRoles, AssignRoles,
                ViewProducts, CreateProducts, UpdateProducts, DeleteProducts, ManageProductInventory,
                ViewOrders, UpdateOrderStatus, CancelOrders, ProcessRefunds,
                ViewShops, CreateShops, UpdateShops, DeleteShops, ManageShopProducts,
                ViewCategories, CreateCategories, UpdateCategories, DeleteCategories,
                ViewSystemLogs, ManageSystemSettings,
                ViewSalesReports, ViewUserReports, ViewSystemReports
            };
        }
    }
}
