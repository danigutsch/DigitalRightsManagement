using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductPublished(Guid ProductId, Guid UserId) : DomainEvent;
