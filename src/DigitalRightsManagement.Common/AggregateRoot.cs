namespace DigitalRightsManagement.Common;

public class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }

    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

    public void QueueDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
