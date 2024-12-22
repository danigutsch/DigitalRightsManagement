using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed record PriceUpdated(Guid ProductId, Price NewPrice, Price OldPrice, string Reason) : DomainEvent;
