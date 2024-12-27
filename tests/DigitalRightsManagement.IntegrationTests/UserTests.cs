using DigitalRightsManagement.Api;
using DigitalRightsManagement.AppHost;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;
using FluentAssertions;
using System.Net.Http.Json;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class UserTests
{
    [Fact]
    public async Task Change_Role_Returns_No_Content()
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

        var admin = SeedData.Users.First(u => u.Role == UserRoles.Admin);
        var target = SeedData.Users.First(u => u.Role == UserRoles.Viewer);
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(admin.Id, target.Id, desiredRole);
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var response = await httpClient.PostAsJsonAsync("/users/change-role", changeRoleDto, cts.Token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}