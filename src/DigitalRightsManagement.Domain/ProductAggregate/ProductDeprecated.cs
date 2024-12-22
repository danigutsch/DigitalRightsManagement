using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductDeprecated(Guid ProductId, Guid UserId) : DomainEvent;
