namespace ECommerceBackend.Domain.Shops;

/// HDHiep - 09/27/2025
/// <summary>
/// Repository interface for managing Shop entities.
/// Provides methods to retrieve and add shops.
/// This interface abstracts the data access layer, allowing for different implementations (e.g., in-memory, database).
/// </summary>
public interface IShopRepository
{
    Task<List<Shop>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Shop?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Shop shop);
}
