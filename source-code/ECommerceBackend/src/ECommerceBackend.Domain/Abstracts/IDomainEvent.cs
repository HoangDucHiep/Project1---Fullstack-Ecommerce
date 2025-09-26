using MediatR;

namespace ECommerceBackend.Domain.Abstracts;


/// HDHiep - 09/24/2025
/// /// <summary>
/// Represents a domain event that occurs within the business domain.
/// Domain events are used to capture and communicate important business occurrences
/// that other parts of the system may need to respond to.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets or sets the unique identifier for this domain event.
    /// This ID can be used for event tracking, deduplication, and correlation.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets or sets the UTC timestamp when this domain event occurred.
    /// This provides a reliable time reference for event ordering and auditing purposes.
    /// </summary>
    DateTimeOffset OccurredOnUtc { get; }
}
