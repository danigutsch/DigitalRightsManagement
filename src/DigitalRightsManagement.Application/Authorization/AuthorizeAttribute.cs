using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AuthorizeAttribute(UserRoles requiredRole) : Attribute
{
    public UserRoles RequiredRole { get; } = requiredRole;
}
