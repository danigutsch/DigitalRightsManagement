using Bogus;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
using DigitalRightsManagement.IntegrationTests.Helpers.HttpAuthHandlers;
using DigitalRightsManagement.MigrationService;

namespace DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;

public abstract class ApiIntegrationTestsBase : IClassFixture<ApiFixture>, IAsyncLifetime, IDisposable
{
    private readonly IServiceScope _scope;

    protected readonly Faker Faker = new();

    protected readonly ApiFixture Fixture;

    internal ManagementDbContext DbContext { get; }

    protected ApiIntegrationTestsBase(ApiFixture fixture)
    {
        Fixture = fixture;
        _scope = Fixture.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
    }

    protected HttpClient GetHttpClient(Agent agent)
    {
        var handler = new BasicAuthHandler(agent.Username, SeedData.Passwords[agent.Id]);
        var client = Fixture.CreateDefaultClient(handler);
        return client;
    }
    public async Task InitializeAsync()
    {
        using var scope = Fixture.Services.CreateScope();

        var applicationDbManager = scope.ServiceProvider.GetRequiredService<IApplicationDbManager>();
        var identityDbManager = scope.ServiceProvider.GetRequiredService<IIdentityDbManager>();

        applicationDbManager.SetSeedData(SeedData.Agents, SeedData.Products);
        identityDbManager.SetSeedData(SeedData.AgentsAndPasswords);

        await applicationDbManager.ResetState(CancellationToken.None);
        await identityDbManager.ResetState(CancellationToken.None)
            .ConfigureAwait(false);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) => _scope.Dispose();
}
