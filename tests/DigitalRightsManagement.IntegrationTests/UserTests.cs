using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
using FluentAssertions;
using System.Net.Http.Json;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class UserTests(ApiFixture fixture) : ApiIntegrationTestsBase(fixture)
{
    [Fact]
    public async Task Get_Current_User_Returns_Success()
    {
        // Arrange
        var user = UserFactory.Seeded();

        // Act
        var response = await GetHttpClient(user).GetAsync("/users/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        userDto.Should().NotBeNull();
        userDto.Id.Should().Be(user.Id);
        userDto.Username.Should().Be(user.Username);
        userDto.Email.Should().Be(user.Email);
        userDto.Role.Should().Be(user.Role);
    }

    [Fact]
    public async Task Change_Role_Returns_Success()
    {
        // Arrange
        var admin = UserFactory.Seeded(UserRoles.Admin);
        var target = UserFactory.Seeded(UserRoles.Viewer);
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(target.Id, desiredRole);

        // Act
        var response = await GetHttpClient(admin).PostAsJsonAsync("users/change-role", changeRoleDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        target = await DbContext.Users.FindAsync(target.Id);
        target.Should().NotBeNull();
        target.Role.Should().Be(desiredRole);
    }

    [Fact]
    public async Task Change_Email_Returns_Success()
    {
        // Arrange
        var user = UserFactory.Seeded();
        var newEmail = Faker.Internet.Email();

        var changeEmailDto = new ChangeEmailDto(newEmail);

        // Act
        var response = await GetHttpClient(user).PostAsJsonAsync("/users/change-email", changeEmailDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        user = await DbContext.Users.FindAsync(user.Id);
        user.Should().NotBeNull();
        user.Email.Should().Be(newEmail);
    }
}
