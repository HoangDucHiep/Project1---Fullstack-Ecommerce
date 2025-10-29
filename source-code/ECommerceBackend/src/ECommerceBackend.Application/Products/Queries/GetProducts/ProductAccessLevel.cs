namespace ECommerceBackend.Application.Products.Queries.GetProducts;

/// <summary>
/// Access level for product queries
/// </summary>
public enum ProductAccessLevel
{
    /// <summary>
    /// Public access - only active products visible
    /// </summary>
    Public,

    /// <summary>
    /// Seller access - can see all products in their shop
    /// </summary>
    Seller,

    /// <summary>
    /// Admin access - can see all products from all shops
    /// </summary>
    Admin
}
