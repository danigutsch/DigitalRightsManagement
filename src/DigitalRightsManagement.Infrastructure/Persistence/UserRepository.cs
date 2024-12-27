using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var users = await context.Users.ToArrayAsync(cancellationToken);

        var ids = users.Select(u => u.Id).ToArray();
        var a = ids.FirstOrDefault(id2 => id2 == id);

        Debug.Assert(a == id);

        var user = await context.Users.FindAsync([id], cancellationToken: cancellationToken);
        return user ?? Result<User>.NotFound();
    }
}
