namespace ECommerceBackend.Common.Domain;


/// <summary>
/// Abstract base class for all domain entities in the e-commerce system.
/// Provides common functionality for domain event handling and entity management.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Initializes a new instance of the Entity class.
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Gets a read-only collection of domain events that have been raised by this entity.
    /// These events represent important business occurrences that may need to be processed
    /// by other parts of the system.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all domain events from this entity.
    /// This is typically called after the events have been processed and published.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Raises a domain event for this entity.
    /// The event will be added to the collection of domain events and can be retrieved
    /// through the DomainEvents property for later processing.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
