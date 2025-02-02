namespace DigitalRightsManagement.Common.DDD;

public interface IAggregateRoot
{
    IReadOnlyList<DomainEvent> DomainEvents { get; }
    DomainEvent[] PopDomainEvents();
}

public partial class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : struct
{
    protected AggregateRoot(TId id) : base(id) { }

    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    protected void QueueDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public DomainEvent[] PopDomainEvents()
    {
        var domainEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return domainEvents;
    }
}
