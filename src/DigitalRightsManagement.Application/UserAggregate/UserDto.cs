using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed record UserDto(Guid Id, string Username, string Email, UserRoles Role, IReadOnlyList<Guid> Products);
