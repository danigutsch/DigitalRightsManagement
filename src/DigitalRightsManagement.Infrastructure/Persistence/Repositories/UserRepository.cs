using Ardalis.Result;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public IUnitOfWork UnitOfWork => context;

    public void Add(User user) => context.Users.Add(user);

    public async Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync([id], cancellationToken: cancellationToken);
        return user ?? Result<User>.NotFound();
    }
}
