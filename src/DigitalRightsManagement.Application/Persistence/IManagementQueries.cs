using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IManagementQueries
{
    Task<Result<IReadOnlyList<Product>>> GetProductsByAgentId(Guid agentId, CancellationToken cancellationToken);
}
