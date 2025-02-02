using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Caching.Repositories;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.Tests.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Shouldly;
using Xunit.Abstractions;

namespace DigitalRightsManagement.IntegrationTests.Caching.Repositories;

public sealed class CachedOwnershipServiceTests : ApiIntegrationTestsBase
{
    private readonly IServiceScope _scope;
    private readonly IDistributedCache _cache;
    private readonly IOwnershipService _ownershipService;

    public CachedOwnershipServiceTests(ITestOutputHelper outputHelper, ApiFixture fixture) : base(outputHelper, fixture)
    {
        _scope = Fixture.Services.CreateScope();
        _cache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        _ownershipService = _scope.ServiceProvider.GetRequiredService<IOwnershipService>();
    }

    [Fact]
    public async Task Caches_Validation_For_Resource()
    {
        // Arrange
        var product = SeedData.Products.Random();

        // Act
        _ = await _ownershipService.IsResourceOwner(product.AgentId.Value, typeof(Product), [product.Id.Value], CancellationToken.None);

        // Assert
        var cacheKey = CachedOwnershipService.GetCacheKey(product.AgentId.Value, typeof(Product), [product.Id.Value]);
        var cached = await _cache.GetAsync(cacheKey);
        cached.ShouldNotBeNull();

        // Act
        _ = await _ownershipService.IsResourceOwner(product.AgentId.Value, typeof(Product), [product.Id.Value], CancellationToken.None);

        // Assert
        var cachedAgain = await _cache.GetAsync(cacheKey);
        cachedAgain.ShouldNotBeNull();
    }

    [Fact]
    public async Task Caches_Validation_For_Multiple_Resources()
    {
        // Arrange
        var productGroupings = SeedData.Products
            .GroupBy(p => p.AgentId)
            .Where(g => g.Count() > 1)
            .Random();

        var ownerId = productGroupings.Key;
        Guid[] resourceIds = [.. productGroupings.Select(p => p.Id.Value)];

        // Act
        _ = await _ownershipService.IsResourceOwner(ownerId.Value, typeof(Product), resourceIds, CancellationToken.None);

        // Assert
        var cacheKey = CachedOwnershipService.GetCacheKey(ownerId.Value, typeof(Product), resourceIds);
        var cached = await _cache.GetAsync(cacheKey);
        cached.ShouldNotBeNull();

        // Act
        _ = await _ownershipService.IsResourceOwner(ownerId.Value, typeof(Product), resourceIds, CancellationToken.None);

        // Assert
        var cachedAgain = await _cache.GetAsync(cacheKey);
        cachedAgain.ShouldNotBeNull();
    }

    protected override void Dispose(bool disposing)
    {
        _scope.Dispose();

        base.Dispose(disposing);
    }
}
