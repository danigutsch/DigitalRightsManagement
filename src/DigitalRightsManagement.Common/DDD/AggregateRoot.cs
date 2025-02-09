namespace DigitalRightsManagement.Common.DDD;

public interface IAggregateRoot
{
    /// <summary>
    /// Gets the list of domain events raised by this aggregate.
    /// </summary>
    IReadOnlyList<DomainEvent> DomainEvents { get; }

    /// <summary>
    /// Removes and returns all pending domain events.
    /// </summary>
    /// <returns>An array of pending domain events.</returns>
    DomainEvent[] PopDomainEvents();
}

/// <summary>
/// Base class for aggregate roots in the domain model.
/// </summary>
/// <typeparam name="TId">The type of the aggregate's identifier.</typeparam>
public class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : struct
{
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    /// <summary>
    /// Initializes a new instance of the aggregate root with the specified ID.
    /// </summary>
    /// <param name="id">The aggregate's identifier.</param>
    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>
    /// Protected constructor for EF Core materialization.
    /// </summary>
    protected AggregateRoot() { }

    /// <summary>
    /// Adds a domain event to the aggregate's collection of pending events.
    /// </summary>
    /// <param name="domainEvent">The domain event to queue.</param>
    protected void QueueDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public DomainEvent[] PopDomainEvents()
    {
        var domainEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return domainEvents;
    }
}
