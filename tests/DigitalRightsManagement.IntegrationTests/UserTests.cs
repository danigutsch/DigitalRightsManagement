using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
using System.Net.Http.Json;
using Shouldly;

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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        userDto.ShouldNotBeNull();
        userDto.Id.ShouldBe(user.Id);
        userDto.Username.ShouldBe(user.Username);
        userDto.Email.ShouldBe(user.Email);
        userDto.Role.ShouldBe(user.Role);
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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        target = await DbContext.Users.FindAsync(target.Id);
        target.ShouldNotBeNull();
        target.Role.ShouldBe(desiredRole);
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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        user = await DbContext.Users.FindAsync(user.Id);
        user.ShouldNotBeNull();
        user.Email.ShouldBe(newEmail);
    }
}
