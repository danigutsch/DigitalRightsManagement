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
    public async Task Get_Products_Happy_Path()
    {
        // Arrange
        var managerWithProducts = UserFactory.Seeded(user => user.Products.Count > 0);

        // Act
        var response = await GetHttpClient(managerWithProducts).GetAsync("products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();

        var productNames = await DbContext.Products
            .Where(p => p.UserId == managerWithProducts.Id)
            .Select(p => p.Name)
            .ToArrayAsync();

        products.Should().NotBeNullOrEmpty();
        products.Select(p => p.Name).Should().BeEquivalentTo(productNames);
    }

    [Fact]
    public async Task Add_Product_Happy_Path()
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

    [Fact]
    public async Task Update_Price_Happy_Path()
    {
        // Arrange
        var manager = UserFactory.Seeded(user => user.Products.Count > 0);
        var productId = manager.Products[0];

        var newPrice = Faker.Random.Decimal(1, 100);
        var newCurrency = Faker.PickRandom<Currency>();
        var reason = Faker.Lorem.Sentence();

        var updatePriceDto = new UpdatePriceDto(newPrice, newCurrency, reason);

        // Act
        var response = await GetHttpClient(manager).PutAsJsonAsync($"/products/{productId}/price", updatePriceDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProduct = await DbContext.Products.FindAsync(productId);
        updatedProduct.Should().NotBeNull();
        updatedProduct.Price.Amount.Should().Be(newPrice);
        updatedProduct.Price.Currency.Should().Be(newCurrency);
    }

    [Fact]
    public async Task Update_Description_Happy_Path()
    {
        // Arrange
        var manager = UserFactory.Seeded(user => user.Products.Count > 0);
        var productId = manager.Products[0];

        var newDescription = Faker.Commerce.ProductDescription();

        var updatePriceDto = new UpdateDescriptionDto(newDescription);

        // Act
        var response = await GetHttpClient(manager).PutAsJsonAsync($"/products/{productId}/description", updatePriceDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProduct = await DbContext.Products.FindAsync(productId);
        updatedProduct.Should().NotBeNull();
        updatedProduct.Description.Should().Be(newDescription);
    }

    [Fact]
    public async Task Publish_Product_Happy_Path()
    {
        // Arrange
        var manager = UserFactory.Seeded(user => user.Products.Count > 0);
        var product = await DbContext.Products.FirstAsync(p => p.UserId == manager.Id && p.Status == ProductStatus.Development);

        // Act
        var response = await GetHttpClient(manager).PostAsync($"/products/{product.Id}/publish", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await DbContext.Entry(product).ReloadAsync();
        product.Should().NotBeNull();
        product.Status.Should().Be(ProductStatus.Published);
    }

    [Fact]
    public async Task Obsolete_Product_Happy_Path()
    {
        // Arrange
        var manager = UserFactory.Seeded(user => user.Products.Count > 0);
        var product = await DbContext.Products.FirstAsync(p => p.UserId == manager.Id);

        // Act
        var response = await GetHttpClient(manager).PostAsync($"/products/{product.Id}/obsolete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await DbContext.Entry(product).ReloadAsync();
        product.Should().NotBeNull();
        product.Status.Should().Be(ProductStatus.Obsolete);
    }
}
