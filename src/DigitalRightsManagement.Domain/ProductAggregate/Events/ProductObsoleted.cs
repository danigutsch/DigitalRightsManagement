using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductObsoleted(ProductId ProductId, AgentId AgentId) : DomainEvent;
