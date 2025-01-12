using Aspire.Hosting;
using Bogus;
using DigitalRightsManagement.AppHost;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests;

public abstract class IntegrationTestsBase(ITestOutputHelper outputHelper) : IAsyncLifetime
{
    private DistributedApplication _app = null!;
    private ApplicationDbContext _dbContext = null!;

    protected DbSet<Product> Products => _dbContext.Products;
    protected DbSet<User> Users => _dbContext.Users;
    protected HttpClient HttpClient { get; private set; } = null!;
    protected Faker Faker { get; } = new();

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DigitalRightsManagement_AppHost>();

        appHost.Services.AddLogging(builder => builder.AddXUnit(outputHelper));

        _app = await appHost.BuildAsync();
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        await _app.StartAsync();

        HttpClient = _app.CreateHttpClient(ResourceNames.Api, "https");

        _dbContext = await CreateApplicationDbContext();

        await resourceNotificationService
            .WaitForResourceAsync(ResourceNames.Api, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30))
            .ConfigureAwait(false);
    }

    private async  Task<ApplicationDbContext> CreateApplicationDbContext()
    {
        var connectionString = await _app.GetConnectionStringAsync(ResourceNames.Database);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task DisposeAsync() => await _app.DisposeAsync().ConfigureAwait(false);
}
