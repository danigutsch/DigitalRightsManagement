using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.UserAggregate.Events;

public sealed record UserCreated(Guid Id, string Username, string Email, UserRoles Role) : DomainEvent;
