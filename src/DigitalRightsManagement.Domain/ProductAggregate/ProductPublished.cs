using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductPublished(Guid ProductId, Guid UserId) : DomainEvent;
