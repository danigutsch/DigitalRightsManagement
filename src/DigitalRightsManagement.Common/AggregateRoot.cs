namespace DigitalRightsManagement.Common;

public class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }

    private readonly List<DomainEvent> _domainEvents = [];
}
