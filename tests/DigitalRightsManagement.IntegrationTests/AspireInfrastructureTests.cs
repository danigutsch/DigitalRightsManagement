using Aspire.Hosting;
using DigitalRightsManagement.AppHost;
using FluentAssertions;

namespace DigitalRightsManagement.IntegrationTests;

public class AspireInfrastructureTests
{
    [Fact]
    public async Task Api_Health_Checks_Are_Okay()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DigitalRightsManagement_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        await app.StartAsync();

        // Act
        using var httpClient = app.CreateHttpClient(ResourceNames.Api);
        await resourceNotificationService.WaitForResourceAsync(ResourceNames.Api, KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var livenessResponse = await httpClient.GetAsync(new Uri("/health", UriKind.Relative));
        var readinessResponse = await httpClient.GetAsync(new Uri("/alive", UriKind.Relative));

        // Assert
        livenessResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        readinessResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Applications_Have_Database_Connection_String()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.DigitalRightsManagement_AppHost>();

        await using var app = await appHost.BuildAsync();

        string[] resourceNames = [ResourceNames.Api, ResourceNames.MigrationService];

        var projects = appHost.Resources
            .Where(r => resourceNames.Contains(r.Name))
            .OfType<IResourceWithEnvironment>();

        // Act
        var envVarsTasks = projects.Select(project =>
            project.GetEnvironmentVariableValuesAsync(DistributedApplicationOperation.Publish)).ToArray();

        var envVars = new List<Dictionary<string, string>>(envVarsTasks.Length);
        foreach (var task in envVarsTasks)
        {
            envVars.Add(await task);
        }

        // Assert
        envVars.Should().HaveCount(resourceNames.Length);
        envVars.Should().AllSatisfy(ev => ev.ContainsKey($"ConnectionString__{ResourceNames.Database}"));
    }
}
