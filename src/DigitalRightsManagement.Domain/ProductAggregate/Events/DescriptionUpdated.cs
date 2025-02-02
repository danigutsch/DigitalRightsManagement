using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record DescriptionUpdated(ProductId ProductId, Description NewDescription, Description OldDescription) : DomainEvent;
