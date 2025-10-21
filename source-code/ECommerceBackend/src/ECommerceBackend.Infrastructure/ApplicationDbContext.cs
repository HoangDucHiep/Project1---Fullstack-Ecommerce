using ECommerceBackend.Application.Abstracts.Exceptions;
using ECommerceBackend.Domain.Abstracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace ECommerceBackend.Infrastructure;

/// HDHiep - 09/24/2025
/// <summary>
/// The application's database context, responsible for managing entity sets and persisting changes.
/// Implements the Unit of Work pattern with transaction support.
/// </summary>
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher,
        ILogger<ApplicationDbContext> logger) : base(options)
    {
        _publisher = publisher;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current transaction if one exists.
    /// </summary>
    public IDbContextTransaction? CurrentTransaction { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there is an active transaction.
    /// </summary>
    public bool HasActiveTransaction => CurrentTransaction != null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Set the schema for the application
        modelBuilder.HasDefaultSchema(Schemas.Application);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            // TODO: Uncomment when domain events are fully implemented
            // await PublishDomainEventsAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency exception occurred in ApplicationDbContext");
            throw new ConcurrencyException(
                new Error("ConcurrencyException", "Concurrency exception occurred in ApplicationDbContext", ErrorType.Conflict),
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving changes in ApplicationDbContext");
            throw;
        }
    }

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous begin operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a transaction is already active.</exception>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already active. Cannot start a new transaction.");
        }

        CurrentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        _logger.LogDebug("Started new transaction with ID: {TransactionId}", CurrentTransaction.TransactionId);

        return CurrentTransaction;
    }

    /// <summary>
    /// Commits the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="InvalidOperationException">Thrown when no active transaction exists.</exception>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await SaveChangesAsync(cancellationToken);
            await CurrentTransaction.CommitAsync(cancellationToken);
            _logger.LogDebug("Committed transaction with ID: {TransactionId}", CurrentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while committing transaction with ID: {TransactionId}", CurrentTransaction.TransactionId);
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                CurrentTransaction.Dispose();
                CurrentTransaction = null;
            }
        }
    }

    /// <summary>
    /// Rolls back the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentTransaction == null)
        {
            _logger.LogWarning("Attempted to rollback transaction, but no active transaction exists.");
            return;
        }

        try
        {
            await CurrentTransaction.RollbackAsync(cancellationToken);
            _logger.LogDebug("Rolled back transaction with ID: {TransactionId}", CurrentTransaction.TransactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rolling back transaction with ID: {TransactionId}", CurrentTransaction.TransactionId);
            throw;
        }
        finally
        {
            CurrentTransaction.Dispose();
            CurrentTransaction = null;
        }
    }

    /// <summary>
    /// Executes a function within a database transaction asynchronously.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The function to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ApplicationArgumentException(Error.Failure(
                "Argument.Null",
                "The provided function to execute in transaction cannot be null. - " + nameof(func)
            ));
        }

        // If there's already an active transaction, execute within it
        if (HasActiveTransaction)
        {
            _logger.LogDebug("Executing function within existing transaction with ID: {TransactionId}", CurrentTransaction!.TransactionId);
            return await func();
        }

        // Start a new transaction
        await using IDbContextTransaction transaction = await BeginTransactionAsync(cancellationToken);

        try
        {
            _logger.LogDebug("Executing function within new transaction with ID: {TransactionId}", transaction.TransactionId);
            T? result = await func();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing function in transaction with ID: {TransactionId}", transaction.TransactionId);
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Executes an action within a database transaction asynchronously.
    /// </summary>
    /// <param name="action">The action to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous transaction operation.</returns>
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        if (action is null)
        {
            throw new ApplicationArgumentException(Error.Failure(
                "Argument.Null",
                "The provided function to execute in transaction cannot be null. - " + nameof(action)
            ));
        }

        await ExecuteInTransactionAsync(async () =>
        {
            await action();
            return Task.CompletedTask;
        }, cancellationToken);
    }

    /// <summary>
    /// Override the Dispose method to properly clean up transactions
    /// </summary>
    public override void Dispose()
    {
        CurrentTransaction?.Dispose();
        base.Dispose();
    }

    /// <summary>
    /// Override the DisposeAsync method to properly clean up transactions
    /// </summary>
    public override async ValueTask DisposeAsync()
    {
        if (CurrentTransaction != null)
        {
            await CurrentTransaction.DisposeAsync();
            CurrentTransaction = null;
        }
        await base.DisposeAsync();
    }

    //private async Task PublishDomainEventsAsync()
    //{
    //    var domainEvents = ChangeTracker
    //        .Entries<Entity>()
    //        .Select(entry => entry.Entity)
    //        .SelectMany(entity =>
    //        {
    //            var domainEvents = entity.GetDomainEvents();
    //            entity.ClearDomainEvents();
    //            return domainEvents;
    //        })
    //        .ToList();

    //    foreach (var domainEvent in domainEvents)
    //    {
    //        await _publisher.Publish(domainEvent);
    //    }
    //}
}
