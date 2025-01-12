using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
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
    public async Task Get_Products_Returns_Success()
    {
        // Arrange
        var managerWithProducts = UserFactory.Seeded(user => user.Products.Count > 0);

        // Act
        HttpClient.AddBasicAuth(managerWithProducts);
        var response = await HttpClient.GetAsync("products");

        // Assert
        response.Should().BeSuccessful();
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();
        products.Should().HaveCount(managerWithProducts.Products.Count);
    }

    [Fact]
    public async Task Add_Product_Returns_Success()
    {
        // Arrange
        var productName = Faker.Commerce.ProductName();
        var productDescription = Faker.Commerce.ProductDescription();
        var productPrice = Faker.Random.Decimal(1, 100);
        var productCurrency = Faker.PickRandom<Currency>();

        var manager = UserFactory.Seeded(UserRoles.Manager);

        var createProductDto = new CreateProductDto(productName, productDescription, productPrice, productCurrency);

        // Act
        HttpClient.AddBasicAuth(manager);
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
