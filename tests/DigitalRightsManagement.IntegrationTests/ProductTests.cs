using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace DigitalRightsManagement.IntegrationTests;

public sealed class ProductTests(ApiFixture fixture) : ApiIntegrationTestsBase(fixture)
{
    [Fact]
    public async Task Get_Products_Returns_Success()
    {
        // Arrange
        var managerWithProducts = UserFactory.Seeded(user => user.Products.Count > 0);

        // Act
        var response = await GetHttpClient(managerWithProducts).GetAsync("products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();

        var productNames = await DbContext.Products
            .Where(p => p.Manager == managerWithProducts.Id)
            .Select(p => p.Name)
            .ToArrayAsync();

        products.Should().NotBeNullOrEmpty();
        products.Select(p => p.Name).Should().BeEquivalentTo(productNames);
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
        var response = await GetHttpClient(manager).PostAsJsonAsync("/products/create", createProductDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var createdProductId = await response.Content.ReadFromJsonAsync<Guid>();
        createdProductId.Should().NotBeEmpty();

        var product = await DbContext.Products.FindAsync(createdProductId);
        product.Should().NotBeNull();
        product.Name.Should().Be(productName);
        product.Description.Should().Be(productDescription);
        product.Price.Amount.Should().Be(productPrice);
        product.Price.Currency.Should().Be(productCurrency);

        manager = await DbContext.Users.FindAsync([manager.Id]);
        manager.Should().NotBeNull();
        manager.Products.Should().Contain(productId => productId == createdProductId);
    }
}
