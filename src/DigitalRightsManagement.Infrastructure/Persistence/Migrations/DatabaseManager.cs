using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalRightsManagement.Infrastructure.Persistence.Migrations;

internal sealed class DatabaseManager(ApplicationDbContext context) : IDatabaseManager
{
    public async Task EnsureDatabase(CancellationToken ct)
    {
        var dbCreator = context.GetService<IRelationalDatabaseCreator>();

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync(ct))
            {
                await dbCreator.CreateAsync(ct);
            }
        });
    }

    public async Task InitializeDatabase(IEnumerable<User> users, CancellationToken ct)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            await context.Database.MigrateAsync(ct);

            var users = await context.Users.ToArrayAsync();

            context.Users.AddRange(users);

            await transaction.CommitAsync(ct);
        });
    }
}

public interface IDatabaseManager
{
    Task EnsureDatabase(CancellationToken ct);
    Task InitializeDatabase(IEnumerable<User> users, CancellationToken ct);
}
