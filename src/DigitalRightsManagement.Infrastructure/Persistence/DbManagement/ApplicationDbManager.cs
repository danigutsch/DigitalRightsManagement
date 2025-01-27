using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IApplicationDbManager : IDatabaseManager
{
    void SetSeedData(IEnumerable<Agent> agents, IEnumerable<Product> products);
}

internal sealed class ApplicationDbManager(ManagementDbContext dbContext) : DatabaseManager<ManagementDbContext>(dbContext), IApplicationDbManager
{
    private Agent[] _agents = [];
    private Product[] _products = [];

    public override async Task SeedDatabase(CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_agents.Length);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_products.Length);

        var strategy = Context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(ct);

            Context.Agents.AddRange(_agents);
            Context.Products.AddRange(_products);

            await Context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }).ConfigureAwait(false);
    }

    public void SetSeedData(IEnumerable<Agent> agents, IEnumerable<Product> products)
    {
        _agents = [.. agents];
        _products = [.. products];
    }
}
