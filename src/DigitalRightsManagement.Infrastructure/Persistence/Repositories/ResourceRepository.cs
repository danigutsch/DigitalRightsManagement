using DigitalRightsManagement.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence.Repositories;

internal class ResourceRepository(ManagementDbContext dbContext) : IResourceRepository
{
    public async Task<bool> IsResourceOwner(Guid ownerId, Type resourceType, Guid[] resourceIds, CancellationToken ct)
    {
        var entityType = dbContext.Model.FindEntityType(resourceType)
                         ?? throw new InvalidOperationException($"Type {resourceType.Name} is not an entity type");

        _ = entityType.FindProperty("AgentId")
            ?? throw new InvalidOperationException($"Entity type {entityType.Name} does not have a AgentId property");

        var table = dbContext.GetType().GetProperty(entityType.ClrType.Name + "s")?.GetValue(dbContext)
                    ?? throw new InvalidOperationException($"DbSet for {entityType.Name} not found in DbContext");

        var query = (table as IQueryable<object>)
                    ?? throw new InvalidOperationException($"Could not get IQueryable for {entityType.Name}");

        var unauthorized = await query
            .Where(e => resourceIds.Contains(EF.Property<Guid>(e, "Id")))
            .AnyAsync(e => EF.Property<Guid>(e, "AgentId") != ownerId, ct);

        return !unauthorized;
    }
}
