using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IDatabaseManager
{
    Task EnsureDatabase(CancellationToken ct);
    Task RunMigration(CancellationToken ct);
    Task SeedDatabase(CancellationToken ct);
}

public abstract class DatabaseManager<TDbContext>(TDbContext context) : IDatabaseManager where TDbContext : DbContext
{
    protected TDbContext Context { get; } = context;

    public async Task EnsureDatabase(CancellationToken ct)
    {
        var dbCreator = Context.GetService<IRelationalDatabaseCreator>();

        var strategy = Context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync(ct))
            {
                await dbCreator.CreateAsync(ct);
            }
        }).ConfigureAwait(false);
    }

    public async Task RunMigration(CancellationToken ct) => await Context.Database.MigrateAsync(ct).ConfigureAwait(false);

    public abstract Task SeedDatabase(CancellationToken ct);
}
