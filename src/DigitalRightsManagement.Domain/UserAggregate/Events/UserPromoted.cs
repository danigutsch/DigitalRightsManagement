using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record UserPromoted(Guid AdminId, Guid ManagerId, UserRoles OldRole, UserRoles NewRole) : DomainEvent;
