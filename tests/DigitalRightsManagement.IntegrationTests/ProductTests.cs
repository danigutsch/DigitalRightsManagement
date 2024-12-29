using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.MigrationService;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class ProductTests(ITestOutputHelper outputHelper) : IntegrationTestsBase(outputHelper)
{
    [Fact]
    public async Task Add_Product_Returns_Success()
    {
        // Arrange
        var productName = Faker.Commerce.ProductName();
        var productDescription = Faker.Commerce.ProductDescription();
        var productPrice = Faker.Random.Decimal(1, 100);
        var productCurrency = Faker.PickRandom<Currency>();

        var managerId = SeedData.ManagerIds[0];

        var createProductDto = new CreateProductDto(managerId, productName, productDescription, productPrice, productCurrency);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products/create", createProductDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
