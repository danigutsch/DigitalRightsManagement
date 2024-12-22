using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductOutOfSupport(Guid ProductId, Guid UserId) : DomainEvent;
