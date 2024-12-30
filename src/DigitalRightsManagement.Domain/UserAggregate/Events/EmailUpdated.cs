using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record EmailUpdated(Guid Id, string NewEmail) : DomainEvent;
