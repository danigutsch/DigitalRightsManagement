using Bogus;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Persistence;
using DigitalRightsManagement.Infrastructure.Persistence.DbManagement;
using DigitalRightsManagement.IntegrationTests.Helpers.HttpAuthHandlers;
using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.Tests.Shared.Logging;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;

public abstract class ApiIntegrationTestsBase : IClassFixture<ApiFixture>, IAsyncLifetime, IDisposable
{
    private readonly IServiceScope _scope;

    protected readonly Faker Faker = new();

    protected readonly WebApplicationFactory<Program> Fixture;

    internal ManagementDbContext DbContext { get; }

    protected ApiIntegrationTestsBase(ITestOutputHelper outputHelper, ApiFixture fixture)
    {
        Fixture = fixture.WithTestLogging(outputHelper);
        _scope = Fixture.Services.CreateScope();

        DbContext = _scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
    }

    protected HttpClient GetHttpClient(Agent agent)
    {
        var handler = new BasicAuthHandler(agent.Username.Value, SeedData.Passwords[agent.Id]);
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

    public async Task DisposeAsync() => await Fixture.DisposeAsync().ConfigureAwait(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        Fixture.Dispose();

        _scope.Dispose();
    }
}
