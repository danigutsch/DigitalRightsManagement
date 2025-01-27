using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record WorkerUnassigned(Guid ProductId, Guid UnassignedBy, Guid WorkerId) : DomainEvent;
