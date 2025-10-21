namespace ECommerceBackend.Domain.Abstracts;

/// HDHiep - 10/21/2025
/// <summary>
/// Interface for advanced transaction management services.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Executes an operation within a transaction with retry logic.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes multiple operations within a single transaction.
    /// </summary>
    /// <param name="operations">List of operations to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the batch operation.</returns>
    Task ExecuteBatchAsync(IEnumerable<Func<Task>> operations, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a savepoint within the current transaction.
    /// </summary>
    /// <param name="savepointName">Name of the savepoint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the savepoint creation.</returns>
    Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back to a specific savepoint.
    /// </summary>
    /// <param name="savepointName">Name of the savepoint to roll back to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the rollback operation.</returns>
    Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default);
}
