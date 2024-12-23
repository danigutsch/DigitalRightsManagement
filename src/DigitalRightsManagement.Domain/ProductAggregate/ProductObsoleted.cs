using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record ProductObsoleted(Guid ProductId, Guid UserId) : DomainEvent;
