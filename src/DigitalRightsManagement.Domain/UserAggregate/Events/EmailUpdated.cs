using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record EmailUpdated(Guid Id, string NewEmail) : DomainEvent;
