using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record ManagerCreated(Guid AdminId, Guid ManagerId) : DomainEvent;
