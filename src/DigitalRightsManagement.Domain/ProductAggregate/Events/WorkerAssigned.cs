using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record WorkerAssigned(Guid ProductId, Guid AssignedBy, Guid WorkerId) : DomainEvent;
