using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class UserTests(ITestOutputHelper outputHelper) : IntegrationTestsBase(outputHelper)
{
    [Fact]
    public async Task Get_Products_Returns_Success()
    {
        // Arrange
        var managerWithProducts = UserFactory.Seeded(user => user.Products.Count > 0);

        // Act
        var response = await HttpClient.GetAsync($"/users/{managerWithProducts.Id}/projects");

        // Assert
        response.Should().BeSuccessful();
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();
        products.Should().HaveCount(managerWithProducts.Products.Count);
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
        var response = await HttpClient.PostAsJsonAsync($"/users/{admin.Id}/change-role", changeRoleDto);

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
        var response = await HttpClient.PostAsJsonAsync($"/users/{user.Id}/change-email", changeEmailDto);

        // Assert
        response.Should().BeSuccessful();

        user = await Users.FindAsync(user.Id);
        user.Should().NotBeNull();
        user!.Email.Should().Be(newEmail);
    }
}
