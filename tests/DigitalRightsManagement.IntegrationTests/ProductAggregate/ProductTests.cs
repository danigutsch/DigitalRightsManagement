using DigitalRightsManagement.Api.Endpoints;
using DigitalRightsManagement.Application.ProductAggregate;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.Tests.Shared.Factories;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests.ProductAggregate;

public sealed class ProductTests(ITestOutputHelper outputHelper, ApiFixture fixture) : ApiIntegrationTestsBase(outputHelper, fixture)
{
    [Fact]
    public async Task Get_Products_Happy_Path()
    {
        // Arrange
        var managerWithProducts = AgentFactory.Seeded(agent => agent.Products.Count > 0 && agent.Role == AgentRoles.Manager);

        // Act
        var response = await GetHttpClient(managerWithProducts).GetAsync("products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<ProductDto[]>();

        var productNames = await DbContext.Products
            .Where(p => p.AgentId == managerWithProducts.Id)
            .Select(p => p.Name.Value)
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
        product.Name.Value.ShouldBe(productName);
        product.Description.Value.ShouldBe(productDescription);
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
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0 && agent.Role == AgentRoles.Manager);
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
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0 && agent.Role == AgentRoles.Manager);
        var productId = manager.Products[0];

        var newDescription = Faker.Commerce.ProductDescription();

        var updatePriceDto = new UpdateDescriptionDto(newDescription);

        // Act
        var response = await GetHttpClient(manager).PutAsJsonAsync($"/products/{productId}/description", updatePriceDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedProduct = await DbContext.Products.FindAsync(productId);
        updatedProduct.ShouldNotBeNull();
        updatedProduct.Description.Value.ShouldBeEquivalentTo(newDescription);
    }

    [Fact]
    public async Task Publish_Product_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0 && agent.Role == AgentRoles.Manager);
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
        var manager = AgentFactory.Seeded(agent => agent.Products.Count > 0 && agent.Role == AgentRoles.Manager);
        var product = await DbContext.Products.FirstAsync(p => p.AgentId == manager.Id);

        // Act
        var response = await GetHttpClient(manager).PostAsync($"/products/{product.Id}/obsolete", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await DbContext.Entry(product).ReloadAsync();
        product.ShouldNotBeNull();
        product.Status.ShouldBe(ProductStatus.Obsolete);
    }

    [Fact]
    public async Task Assign_Worker_Happy_Path()
    {
        // Arrange
        var manager = AgentFactory.Seeded(user => user.Products.Count > 0 && user.Role == AgentRoles.Manager);
        var productId = manager.Products[0];
        var worker = AgentFactory.Seeded(worker => !worker.Products.Contains(productId));

        var assignWorkerDto = new AssignWorkerDto(worker.Id);

        // Act
        var response = await GetHttpClient(manager)
            .PostAsJsonAsync($"/products/{productId}/workers", assignWorkerDto);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var product = await DbContext.Products.FindAsync(productId);
        product.ShouldNotBeNull();
        product.AssignedWorkers.ShouldContain(worker.Id);

        worker = await DbContext.Agents.FindAsync(worker.Id);
        worker.ShouldNotBeNull();
        worker.Products.ShouldContain(productId);
    }

    [Fact]
    public async Task Unassign_Worker_Happy_Path()
    {
        // Arrange
        var product = SeedData.Products.First(p => p.AssignedWorkers.Count > 0);
        var manager = SeedData.Agents.First(a => a.Id == product.AgentId);
        var workerId = product.AssignedWorkers[0];

        // Act
        var response = await GetHttpClient(manager).DeleteAsync($"/products/{product.Id}/workers/{workerId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        product = await DbContext.Products.FindAsync(product.Id);
        product.ShouldNotBeNull();
        product.AssignedWorkers.ShouldNotContain(workerId);

        var worker = await DbContext.Agents.FindAsync(workerId);
        worker.ShouldNotBeNull();
        worker.Products.ShouldNotContain(product.Id);
    }
}
