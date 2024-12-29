using DigitalRightsManagement.Api.Endpoints;
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
        var adminId = SeedData.AdminIds[0];
        var targetId = SeedData.ViewerIds[0];
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(adminId, targetId, desiredRole);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/users/change-role", changeRoleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Change_Email_Returns_Success()
    {
        // Arrange
        var userId = SeedData.ViewerIds[0];
        var newEmail = Faker.Internet.Email();

        var changeEmailDto = new ChangeEmailDto(userId, newEmail);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/users/change-email", changeEmailDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
