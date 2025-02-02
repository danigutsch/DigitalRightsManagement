using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record WorkerUnassigned(ProductId ProductId, AgentId UnassignedBy, AgentId WorkerId) : DomainEvent;
