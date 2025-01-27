using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Infrastructure.Caching.Repositories;
using DigitalRightsManagement.IntegrationTests.Helpers.Abstractions;
using DigitalRightsManagement.MigrationService;
using DigitalRightsManagement.Tests.Shared;
using Microsoft.Extensions.Caching.Distributed;
using Shouldly;

namespace DigitalRightsManagement.IntegrationTests.Caching.Repositories;

public sealed class CachedResourceRepositoryTests : ApiIntegrationTestsBase
{
    private readonly IServiceScope _scope;
    private readonly IDistributedCache _cache;
    private readonly IResourceRepository _resourceRepository;

    public CachedResourceRepositoryTests(ApiFixture fixture) : base(fixture)
    {
        _scope = Fixture.Services.CreateScope();
        _cache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        _resourceRepository = _scope.ServiceProvider.GetRequiredService<IResourceRepository>();
    }

    [Fact]
    public async Task Caches_Validation_For_Resource()
    {
        // Arrange
        var product = SeedData.Products.Random();

        // Act
        _ = await _resourceRepository.IsResourceOwner(product.AgentId, typeof(Product), [product.Id], CancellationToken.None);

        // Assert
        var cacheKey = CachedResourceRepository.GetCacheKey(product.AgentId, typeof(Product), [product.Id]);
        var cached = await _cache.GetAsync(cacheKey);
        cached.ShouldNotBeNull();

        // Act
        _ = await _resourceRepository.IsResourceOwner(product.AgentId, typeof(Product), [product.Id], CancellationToken.None);

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
        Guid[] resourceIds = [.. productGroupings.Select(p => p.Id)];

        // Act
        _ = await _resourceRepository.IsResourceOwner(ownerId, typeof(Product), resourceIds, CancellationToken.None);

        // Assert
        var cacheKey = CachedResourceRepository.GetCacheKey(ownerId, typeof(Product), resourceIds);
        var cached = await _cache.GetAsync(cacheKey);
        cached.ShouldNotBeNull();

        // Act
        _ = await _resourceRepository.IsResourceOwner(ownerId, typeof(Product), resourceIds, CancellationToken.None);

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
