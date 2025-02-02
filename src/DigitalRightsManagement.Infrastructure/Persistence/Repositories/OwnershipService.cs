using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DigitalRightsManagement.Infrastructure.Persistence.Repositories;

internal class OwnershipService(ManagementDbContext dbContext, ILogger<OwnershipService> logger) : IOwnershipService
{
    private static readonly Dictionary<Type, Func<ManagementDbContext, Guid, Guid[], Task<Guid[]>>> OwnershipCheckers = new()
    {
        { typeof(Product), CheckProductOwnership }
    };

    public async Task<bool> IsResourceOwner(Guid ownerId, Type resourceType, Guid[] resourceIds, CancellationToken ct)
    {
        if (!OwnershipCheckers.TryGetValue(resourceType, out var resourceChecker))
        {
            throw new NotSupportedException($"Resource type {resourceType.Name} is not supported");
        }

        var notOwnedResources = await resourceChecker(dbContext, ownerId, resourceIds);
        if (notOwnedResources.Length > 0)
        {
            logger.UnauthorizedResourceAccess(resourceType.Name, ownerId, notOwnedResources);
            return false;
        }

        return true;
    }

    private static async Task<Guid[]> CheckProductOwnership(ManagementDbContext dbContext, Guid ownerId, Guid[] productIds)
    {
        var agentId = AgentId.From(ownerId);
        var ids = productIds.Select(ProductId.From).ToArray();

        var notOwnedResourceIds = await dbContext.Set<Product>()
            .Where(p => ids.Contains(p.Id) && agentId != p.AgentId)
            .Select(p => p.Id)
            .ToArrayAsync();

        return [.. notOwnedResourceIds.Select(id => id.Value)];
    }
}

internal static partial class OwnershipServiceLogger
{
    [LoggerMessage(LogLevel.Error, "Unauthorized access to resource {ResourceType} with ID {OwnerId} for products {ProductIds}")]
    public static partial void UnauthorizedResourceAccess(this ILogger<OwnershipService> logger, string resourceType, Guid ownerId, IEnumerable<Guid> productIds);
}
