using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;
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
        var managerWithMostProducts = SeedData.ManagerAndProductIds.MaxBy(kvp => kvp.Value.Length);

        // Act
        var response = await HttpClient.GetAsync($"/users/{managerWithMostProducts.Key}/projects");

        // Assert
        response.Should().BeSuccessful();
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();
        products.Should().HaveCount(3);
    }

    [Fact]
    public async Task Change_Role_Returns_Success()
    {
        // Arrange
        var adminId = SeedData.AdminIds[0];
        var targetId = SeedData.ViewerIds[0];
        const UserRoles desiredRole = UserRoles.Admin;

        var changeRoleDto = new ChangeUserDto(targetId, desiredRole);

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/users/{adminId}/change-role", changeRoleDto);

        // Assert
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Change_Email_Returns_Success()
    {
        // Arrange
        var userId = SeedData.ViewerIds[0];
        var newEmail = Faker.Internet.Email();

        var changeEmailDto = new ChangeEmailDto(newEmail);

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/users/{userId}/change-email", changeEmailDto);

        // Assert
        response.Should().BeSuccessful();
    }
}
