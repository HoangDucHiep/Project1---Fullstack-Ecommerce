using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerceBackend.Domain.Abstracts;



/// HDHiep - 10/21/2025
/// <summary>
/// Defines a contract for a unit of work that encapsulates a series of operations
/// to be committed as a single transaction.
/// </summary>
public interface IUnitOfWork
{

    #region Manual Transaction Management

    /// <summary>
    /// Asynchronously saves all changes made in the context to the underlying database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous begin operation. The task result contains the transaction object.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion

    /// <summary>
    /// Executes a function within a database transaction asynchronously.
    /// If the function executes successfully, the transaction is committed.
    /// If an exception occurs, the transaction is rolled back.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The function to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous transaction operation. The task result contains the function result.</returns>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a database transaction asynchronously.
    /// If the action executes successfully, the transaction is committed.
    /// If an exception occurs, the transaction is rolled back.
    /// </summary>
    /// <param name="action">The action to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current transaction if one exists.
    /// </summary>
    IDbContextTransaction? CurrentTransaction { get; }

    /// <summary>
    /// Gets a value indicating whether there is an active transaction.
    /// </summary>
    bool HasActiveTransaction { get; }

}
