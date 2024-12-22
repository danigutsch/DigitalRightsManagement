namespace DigitalRightsManagement.Common;

public class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }

    private readonly List<DomainEvent> _domainEvents = [];

    public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
