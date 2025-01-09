using Microsoft.AspNetCore.Identity;

namespace DigitalRightsManagement.Infrastructure.Persistence.Identity;

public sealed class BasicAuthUser : IdentityUser
{
    public required Guid DomainUserId { get; init; }
}
