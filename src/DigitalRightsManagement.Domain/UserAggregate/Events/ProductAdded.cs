using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record ProductAdded(Guid UserId, Guid ProductId) : DomainEvent;
