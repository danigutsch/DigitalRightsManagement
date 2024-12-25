using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Infrastructure;

internal class UserRepository(ApplicationDbContext context)
{
    public async Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        return user ?? Result<User>.NotFound();
    }
}
