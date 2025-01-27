using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record EmailUpdated(Guid Id, string NewEmail) : DomainEvent;
