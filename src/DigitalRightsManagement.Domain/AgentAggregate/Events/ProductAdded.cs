using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Domain.AgentAggregate.Events;

public sealed record ProductAdded(AgentId AgentId, ProductId ProductId) : DomainEvent;
