namespace ECommerceBackend.Domain.Abstracts;



/// HDHiep - 09/24/2025
/// <summary>
/// Defines a contract for a unit of work that encapsulates a series of operations
/// to be committed as a single transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made in the context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
