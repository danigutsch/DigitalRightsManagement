using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record EmailUpdated(AgentId Id, EmailAddress NewEmail) : DomainEvent;
