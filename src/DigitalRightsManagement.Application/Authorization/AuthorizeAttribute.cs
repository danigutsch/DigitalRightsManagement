using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AuthorizeAttribute() : Attribute
    {
        public AuthorizeAttribute(UserRoles requiredRole) : this() => RequiredRole = requiredRole;

        public UserRoles? RequiredRole { get; }
    }
}
