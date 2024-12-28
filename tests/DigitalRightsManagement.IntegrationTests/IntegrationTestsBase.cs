using Aspire.Hosting;
using Bogus;
using DigitalRightsManagement.AppHost;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests;

public abstract class IntegrationTestsBase(ITestOutputHelper outputHelper) : IAsyncLifetime
{
    private DistributedApplication _app = null!;
    protected HttpClient HttpClient = null!;
    protected readonly Faker Faker = new();

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DigitalRightsManagement_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        appHost.Services.AddLogging(builder => builder.AddXUnit(outputHelper));

        _app = await appHost.BuildAsync();
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();

        await _app.StartAsync();

        HttpClient = _app.CreateHttpClient(ResourceNames.Api);

        await resourceNotificationService
            .WaitForResourceAsync(ResourceNames.Api, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30))
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync() => await _app.DisposeAsync().ConfigureAwait(false);
}
