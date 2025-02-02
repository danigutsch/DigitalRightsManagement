using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record ProductRemoved(AgentId Id, ProductId ProductId) : DomainEvent;
