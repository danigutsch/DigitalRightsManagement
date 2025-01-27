using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record ProductRemoved(Guid Id, Guid ProductId) : DomainEvent;
