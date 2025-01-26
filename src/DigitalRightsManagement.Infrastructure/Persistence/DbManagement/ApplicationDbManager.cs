using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IApplicationDbManager : IDatabaseManager
{
    void SetSeedData(IEnumerable<User> users, IEnumerable<Product> products);
}

internal sealed class ApplicationDbManager(ManagementDbContext dbContext) : DatabaseManager<ManagementDbContext>(dbContext), IApplicationDbManager
{
    private User[] _users = [];
    private Product[] _products = [];

    public override async Task SeedDatabase(CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_users.Length);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(_products.Length);

        var strategy = Context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(ct);

            Context.Users.AddRange(_users);
            Context.Products.AddRange(_products);

            await Context.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }).ConfigureAwait(false);
    }

    public void SetSeedData(IEnumerable<User> users, IEnumerable<Product> products)
    {
        _users = [.. users];
        _products = [.. products];
    }
}
