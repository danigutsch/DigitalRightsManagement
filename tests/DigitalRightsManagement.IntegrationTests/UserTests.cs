using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
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
        var admin = UserFactory.Seeded(UserRoles.Admin);
        var target = UserFactory.Seeded(UserRoles.Viewer);
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(target.Id, desiredRole);

        // Act
        HttpClient.AddBasicAuth(admin);
        var response = await HttpClient.PostAsJsonAsync("users/change-role", changeRoleDto);

        // Assert
        response.Should().BeSuccessful();

        target = await Users.FindAsync(target.Id);
        target.Should().NotBeNull();
        target!.Role.Should().Be(desiredRole);
    }

    [Fact]
    public async Task Change_Email_Returns_Success()
    {
        // Arrange
        var user = UserFactory.Seeded();
        var newEmail = Faker.Internet.Email();

        var changeEmailDto = new ChangeEmailDto(newEmail);

        // Act
        HttpClient.AddBasicAuth(user);
        var response = await HttpClient.PostAsJsonAsync("/users/change-email", changeEmailDto);

        // Assert
        response.Should().BeSuccessful();

        user = await Users.FindAsync(user.Id);
        user.Should().NotBeNull();
        user!.Email.Should().Be(newEmail);
    }
}
