using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record EmailUpdated(AgentId Id, string NewEmail) : DomainEvent;
