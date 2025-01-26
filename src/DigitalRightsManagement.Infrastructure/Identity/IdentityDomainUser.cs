using Microsoft.AspNetCore.Identity;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal abstract class IdentityDomainUser : IdentityUser
{
    public required Guid DomainUserId { get; init; }
}
