namespace DigitalRightsManagement.Common.DDD;

public partial class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }

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
