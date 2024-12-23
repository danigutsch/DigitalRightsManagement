using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate.Events;

public sealed record DescriptionUpdated(Guid ProductId, string NewDescription, string OldDescription) : DomainEvent;
