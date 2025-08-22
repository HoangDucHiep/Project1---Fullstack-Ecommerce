
using ECommerceBackend.Common.Domain.Utils;

namespace ECommerceBackend.Common.Domain;


/// <summary>
/// Base class for domain events that occur within the business domain.
/// Provides a unique identifier and UTC timestamp for when the event occurred,
/// aligning with the <see cref="IDomainEvent"/> contract.
/// </summary>
/// <remarks>
/// - The parameterless constructor generates a new event ID and sets the occurrence time to <see cref="DateTimeOffset.UtcNow"/>.
/// - The protected constructor with parameters allows reconstruction (e.g., from persistence) with explicit values.
/// </remarks>
/// <seealso cref="IDomainEvent"/>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class with a generated identifier
    /// and the current UTC timestamp.
    /// </summary>
    protected DomainEvent()
    {
        Id = IdGenerator.GenerateId();
        OccurredOnUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class with the specified identifier and occurrence timestamp.
    /// Typically used when rehydrating an event from a persistent store.
    /// </summary>
    /// <param name="id">The unique identifier for the event.</param>
    /// <param name="occurredOnUtc">The UTC timestamp when the event occurred.</param>
    protected DomainEvent(Guid id, DateTimeOffset occurredOnUtc)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
    }

    /// <inheritdoc />
    public Guid Id { get; init; }

    /// <inheritdoc />
    public DateTimeOffset OccurredOnUtc { get; init; }
}
