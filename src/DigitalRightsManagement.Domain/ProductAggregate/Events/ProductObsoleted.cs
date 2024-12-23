using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record ProductObsoleted(Guid ProductId, Guid UserId) : DomainEvent;
