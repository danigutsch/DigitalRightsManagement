using Ardalis.Result;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain.ProductAggregate;
using Microsoft.EntityFrameworkCore;

namespace DigitalRightsManagement.Infrastructure.Persistence;

internal sealed class ManagementQueries(ManagementDbContext dbContext) : IManagementQueries
{
    public async Task<Result<IReadOnlyList<Product>>> GetProductsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .Where(product => product.UserId == userId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}