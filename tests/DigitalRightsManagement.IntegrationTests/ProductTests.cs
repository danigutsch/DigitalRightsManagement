using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
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

        var managerId = UserFactory.Seeded(UserRoles.Manager).Id;

        var createProductDto = new CreateProductDto(managerId, productName, productDescription, productPrice, productCurrency);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products/create", createProductDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        id.Should().NotBeEmpty();

        var product = await Products.FindAsync(id);
        product.Should().NotBeNull();
        product!.Name.Should().Be(productName);
        product.Description.Should().Be(productDescription);
        product.Price.Amount.Should().Be(productPrice);
        product.Price.Currency.Should().Be(productCurrency);
    }
}
