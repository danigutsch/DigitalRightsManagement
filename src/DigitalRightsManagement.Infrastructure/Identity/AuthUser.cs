using Microsoft.AspNetCore.Identity;

namespace DigitalRightsManagement.Infrastructure.Identity;

public sealed class AuthUser : IdentityUser
{
    public required Guid DomainUserId { get; init; }
}
