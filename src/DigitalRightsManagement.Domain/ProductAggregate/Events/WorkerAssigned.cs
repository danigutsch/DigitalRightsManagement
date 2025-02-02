using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record WorkerAssigned(ProductId ProductId, AgentId AssignedBy, AgentId WorkerId) : DomainEvent;
