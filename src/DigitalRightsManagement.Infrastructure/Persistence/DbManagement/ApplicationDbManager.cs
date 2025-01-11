using CommunityToolkit.Diagnostics;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence.DbManagement;

public interface IApplicationDbManager : IDatabaseManager
{
    void SetSeedData(IEnumerable<User> users, IEnumerable<Product> products);
}

internal sealed class ApplicationDbManager(ApplicationDbContext context) : DatabaseManager<ApplicationDbContext>(context), IApplicationDbManager
{
    private User[] _users = [];
    private Product[] _products = [];

    public override async Task SeedDatabase(CancellationToken ct)
    {
        Guard.IsNotEmpty(_users);
        Guard.IsNotEmpty(_products);

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
