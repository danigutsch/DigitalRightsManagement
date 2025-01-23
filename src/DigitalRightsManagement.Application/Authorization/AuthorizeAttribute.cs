using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class AuthorizeAttribute(UserRoles requiredRole) : Attribute
{
    public UserRoles RequiredRole { get; } = requiredRole;
}
