using DigitalRightsManagement.Application.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;

namespace DigitalRightsManagement.Infrastructure.Caching.Repositories;

internal sealed class CachedResourceRepository(
    IResourceRepository inner,
    IDistributedCache cache)
    : IResourceRepository
{
    private const string CacheKeyPrefix = "ownership:";

    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    public async Task<bool> IsResourceOwner(Guid userId, Type resourceType, Guid[] resourceIds, CancellationToken ct)
    {
        var cacheKey = GetCacheKey(userId, resourceType, resourceIds);

        var cached = await cache.GetAsync(cacheKey, ct);
        if (cached != null)
        {
            return BitConverter.ToBoolean(cached);
        }

        var isOwner = await inner.IsResourceOwner(userId, resourceType, resourceIds, ct);

        await cache.SetAsync(
            cacheKey,
            BitConverter.GetBytes(isOwner),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl },
            ct);

        return isOwner;
    }

    private static string GetCacheKey(Guid userId, Type resourceType, Guid[] resourceIds)
    {
        byte[] idsBytes = [.. resourceIds.SelectMany(id => id.ToByteArray())];
        var idsHash = Convert.ToBase64String(SHA256.HashData(idsBytes));
        return $"{CacheKeyPrefix}{userId}:{resourceType.Name}:{idsHash}";
    }
}
