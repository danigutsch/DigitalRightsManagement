using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AuthorizeAttribute() : Attribute
    {
        public AuthorizeAttribute(AgentRoles requiredRole) : this() => RequiredRole = requiredRole;

        public AgentRoles? RequiredRole { get; }
    }
}
