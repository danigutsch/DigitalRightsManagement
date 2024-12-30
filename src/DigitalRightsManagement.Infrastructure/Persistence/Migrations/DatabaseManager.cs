using DigitalRightsManagement.Domain.ProductAggregate;
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
        }).ConfigureAwait(false);
    }

    public async Task RunMigration(CancellationToken ct) => await context.Database.MigrateAsync(ct).ConfigureAwait(false);

    public async Task SeedData(IEnumerable<User> users, IEnumerable<Product> products, CancellationToken ct)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(ct);

            context.Users.AddRange(users);
            context.Products.AddRange(products);

            await context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }).ConfigureAwait(false);
    }
}

public interface IDatabaseManager
{
    Task EnsureDatabase(CancellationToken ct);
    Task RunMigration(CancellationToken ct);
    Task SeedData(IEnumerable<User> users, IEnumerable<Product> products, CancellationToken ct);
}
