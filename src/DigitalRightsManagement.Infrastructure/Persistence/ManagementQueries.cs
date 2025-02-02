using Ardalis.Result;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ManagementQueries(ManagementDbContext dbContext) : IManagementQueries
{
    public async Task<Result<IReadOnlyList<Product>>> GetProductsByAgentId(AgentId agentId, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.AgentId == agentId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
