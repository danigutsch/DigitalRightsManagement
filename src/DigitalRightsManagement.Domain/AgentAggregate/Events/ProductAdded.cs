using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record ProductAdded(Guid AgentId, Guid ProductId) : DomainEvent;
