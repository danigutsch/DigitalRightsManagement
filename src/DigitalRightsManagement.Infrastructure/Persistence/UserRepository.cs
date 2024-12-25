using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken: cancellationToken);
        return user ?? Result<User>.NotFound();
    }
}
