using ECommerceBackend.Domain.Abstracts;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Infrastructure.Transactions;

/// HDHiep - 10/21/2025
/// <summary>
/// Service for managing database transactions with advanced features.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(IUnitOfWork unitOfWork, ILogger<TransactionService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Executes multiple operations within a single transaction with retry logic.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="maxRetries">Maximum number of retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3, CancellationToken cancellationToken = default)
    {
        int attempt = 0;

        while (attempt <= maxRetries)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(operation, cancellationToken);
            }
            catch (Exception ex) when (attempt < maxRetries && IsRetriableException(ex))
            {
                attempt++;
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100); // Exponential backoff

                _logger.LogWarning(ex,
                    "Transaction failed on attempt {Attempt}/{MaxRetries}. Retrying after {Delay}ms",
                    attempt, maxRetries + 1, delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        // This should never be reached, but compiler requires it
        throw new InvalidOperationException("All retry attempts failed");
    }

    /// <summary>
    /// Executes multiple operations within a single transaction.
    /// </summary>
    /// <param name="operations">List of operations to execute.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the transaction.</returns>
    public async Task ExecuteBatchAsync(
        IEnumerable<Func<Task>> operations,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (Func<Task> operation in operations)
            {
                await operation();
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Creates a savepoint within the current transaction.
    /// </summary>
    /// <param name="savepointName">Name of the savepoint.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the savepoint creation.</returns>
    public async Task CreateSavepointAsync(string savepointName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(savepointName))
        {
            throw new ArgumentException("Savepoint name cannot be null or empty", nameof(savepointName));
        }

        if (!_unitOfWork.HasActiveTransaction)
        {
            throw new InvalidOperationException("No active transaction to create savepoint in");
        }

        await _unitOfWork.CurrentTransaction!.CreateSavepointAsync(savepointName, cancellationToken);
        _logger.LogDebug("Created savepoint '{SavepointName}' in transaction {TransactionId}",
            savepointName, _unitOfWork.CurrentTransaction.TransactionId);
    }

    /// <summary>
    /// Rolls back to a specific savepoint.
    /// </summary>
    /// <param name="savepointName">Name of the savepoint to roll back to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the rollback operation.</returns>
    public async Task RollbackToSavepointAsync(string savepointName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(savepointName))
        {
            throw new ArgumentException("Savepoint name cannot be null or empty", nameof(savepointName));
        }

        if (!_unitOfWork.HasActiveTransaction)
        {
            throw new InvalidOperationException("No active transaction to rollback savepoint in");
        }

        await _unitOfWork.CurrentTransaction!.RollbackToSavepointAsync(savepointName, cancellationToken);
        _logger.LogDebug("Rolled back to savepoint '{SavepointName}' in transaction {TransactionId}",
            savepointName, _unitOfWork.CurrentTransaction.TransactionId);
    }

    /// <summary>
    /// Determines if an exception is retriable.
    /// </summary>
    /// <param name="exception">The exception to check.</param>
    /// <returns>True if the exception is retriable, false otherwise.</returns>
    private static bool IsRetriableException(Exception exception)
    {
        // Add logic to determine if the exception is retriable
        // Common retriable exceptions: timeout, deadlock, temporary connection issues
        return exception is TimeoutException ||
               exception.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
               exception.Message.Contains("connection", StringComparison.OrdinalIgnoreCase);
    }
}
