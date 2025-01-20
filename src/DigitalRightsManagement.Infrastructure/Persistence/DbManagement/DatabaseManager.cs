using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IDatabaseManager
{
    Task EnsureDatabase(CancellationToken ct);
    Task RunMigration(CancellationToken ct);
    Task SeedDatabase(CancellationToken ct);
    Task ResetState(CancellationToken ct);

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

    public async Task ResetState(CancellationToken ct)
    {
        await CleanDatabase(ct);
        await SeedDatabase(ct)
            .ConfigureAwait(false);
    }

    private async Task CleanDatabase(CancellationToken ct)
    {
        var strategy = Context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Disable foreign key checks
            await Context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';", ct);

            try
            {
                // Get all tables with their schemas
                var tables = Context.Model.GetEntityTypes()
                    .Select(t => (Schema: t.GetSchema() ?? "public", Table: t.GetTableName()))
                    .Where(t => t.Table != null)
                    .ToList();

                // Delete all data
                foreach (var (schema, table) in tables)
                {
                    var sql = string.Create(CultureInfo.InvariantCulture,
                        $"TRUNCATE TABLE \"{EscapeIdentifier(schema)}\".\"{EscapeIdentifier(table)}\" CASCADE");
                    await Context.Database.ExecuteSqlRawAsync(sql, ct);
                }
            }
            finally
            {
                // Re-enable foreign key checks
                await Context.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';", ct);
            }
        }).ConfigureAwait(false);
    }


    private static string EscapeIdentifier(string? identifier)
    {
        return string.IsNullOrEmpty(identifier)
            ? string.Empty :
            // Replace any quotes with two quotes to escape them
            identifier.Replace("\"", "\"\"", StringComparison.OrdinalIgnoreCase);
    }
}
