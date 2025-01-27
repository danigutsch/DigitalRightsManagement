using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.Tests.Shared.Factories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net.Http.Json;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.IntegrationTests.ProductAggregate;

public sealed class ProductTests(ApiFixture fixture) : ApiIntegrationTestsBase(fixture)
{
    [Fact]
    public async Task Get_Products_Happy_Path()
    {
        // Arrange
        var managerWithProducts = AgentFactory.Seeded(agent => agent.Products.Count > 0);

        // Act
        var response = await GetHttpClient(managerWithProducts).GetAsync("products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();

        var productNames = await DbContext.Products
            .Where(p => p.AgentId == managerWithProducts.Id)
            .Select(p => p.Name)
            .ToArrayAsync();

        products.ShouldNotBeEmpty();
        products.Select(p => p.Name).ToArray().ShouldBeEquivalentTo(productNames);
    }

    [Fact]
    public async Task Add_Product_Happy_Path()
    {
        // Arrange
        var productName = Faker.Commerce.ProductName();
        var productDescription = Faker.Commerce.ProductDescription();
        var productPrice = Faker.Random.Decimal(1, 100);
        var productCurrency = Faker.PickRandom<Currency>();

        var manager = AgentFactory.Seeded(AgentRoles.Manager);

        var createProductDto = new CreateProductDto(productName, productDescription, productPrice, productCurrency);

        // Act
        var response = await GetHttpClient(manager).PostAsJsonAsync("/products/create", createProductDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var createdProductId = await response.Content.ReadFromJsonAsync<Guid>();
        createdProductId.ShouldNotBe(Guid.Empty);

        var product = await DbContext.Products.FindAsync(createdProductId);
        product.ShouldNotBeNull();
        product.Name.ShouldBe(productName);
        product.Description.ShouldBe(productDescription);
        product.Price.Amount.ShouldBe(productPrice);
        product.Price.Currency.ShouldBe(productCurrency);

        manager = await DbContext.Agents.FindAsync(manager.Id);
        manager.ShouldNotBeNull();
        manager.Products.ShouldContain(productId => productId == createdProductId);
    }

    [Fact]
    public async Task Update_Price_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0);
        var productId = manager.Products[0];

        var newPrice = Faker.Random.Decimal(1, 100);
        var newCurrency = Faker.PickRandom<Currency>();
        var reason = Faker.Lorem.Sentence();

        var updatePriceDto = new UpdatePriceDto(newPrice, newCurrency, reason);

        // Act
        var response = await GetHttpClient(manager).PutAsJsonAsync($"/products/{productId}/price", updatePriceDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedProduct = await DbContext.Products.FindAsync(productId);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Price.Amount.ShouldBe(newPrice);
        updatedProduct.Price.Currency.ShouldBe(newCurrency);
    }

    [Fact]
    public async Task Update_Description_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0);
        var productId = manager.Products[0];

        var newDescription = Faker.Commerce.ProductDescription();

        var updatePriceDto = new UpdateDescriptionDto(newDescription);

        // Act
        var response = await GetHttpClient(manager).PutAsJsonAsync($"/products/{productId}/description", updatePriceDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedProduct = await DbContext.Products.FindAsync(productId);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Description.ShouldBe(newDescription);
    }

    [Fact]
    public async Task Publish_Product_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0);
        var product = await DbContext.Products.FirstAsync(p => p.AgentId == manager.Id && p.Status == ProductStatus.Development);

        // Act
        var response = await GetHttpClient(manager).PostAsync($"/products/{product.Id}/publish", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await DbContext.Entry(product).ReloadAsync();
        product.ShouldNotBeNull();
        product.Status.ShouldBe(ProductStatus.Published);
    }

    [Fact]
    public async Task Obsolete_Product_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0);
        var product = await DbContext.Products.FirstAsync(p => p.AgentId == manager.Id);

        // Act
        var response = await GetHttpClient(manager).PostAsync($"/products/{product.Id}/obsolete", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await DbContext.Entry(product).ReloadAsync();
        product.ShouldNotBeNull();
        product.Status.ShouldBe(ProductStatus.Obsolete);
    }
}
