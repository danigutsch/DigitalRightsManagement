using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate;

public sealed record EmailUpdated(Guid Id, string NewEmail) : DomainEvent;
