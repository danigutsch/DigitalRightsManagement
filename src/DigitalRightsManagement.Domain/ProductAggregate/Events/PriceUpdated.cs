using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record PriceUpdated(Guid ProductId, Price NewPrice, Price OldPrice, string Reason) : DomainEvent;
