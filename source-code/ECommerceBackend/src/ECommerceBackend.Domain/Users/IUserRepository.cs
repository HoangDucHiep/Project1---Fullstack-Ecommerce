namespace ECommerceBackend.Domain.Users;

/// HDHiep - 09/24/2025
/// <summary>
/// Repository interface for managing <see cref="User"/> entities.
/// Defines methods for retrieving and adding users.
/// </summary>
public interface IUserRepository
{
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(User user);
}
