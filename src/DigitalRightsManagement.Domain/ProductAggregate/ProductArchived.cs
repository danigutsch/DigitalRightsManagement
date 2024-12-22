using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductArchived(Guid ProductId, Guid UserId) : DomainEvent;
