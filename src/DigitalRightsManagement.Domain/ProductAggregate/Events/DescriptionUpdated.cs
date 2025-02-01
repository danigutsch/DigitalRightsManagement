using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record DescriptionUpdated(Guid ProductId, Description NewDescription, Description OldDescription) : DomainEvent;
