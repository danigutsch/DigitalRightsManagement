using DigitalRightsManagement.Api;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class UserTests(ITestOutputHelper outputHelper) : IntegrationTestsBase(outputHelper)
{
    [Fact]
    public async Task Change_Role_Returns_Success()
    {
        // Arrange
        var admin = SeedData.Users.First(u => u.Role == UserRoles.Admin);
        var target = SeedData.Users.First(u => u.Role == UserRoles.Viewer);
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(admin.Id, target.Id, desiredRole);

        // Act
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        var response = await HttpClient.PostAsJsonAsync("/users/change-role", changeRoleDto, cts.Token);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
