using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IManagementQueries
{
    Task<Result<IReadOnlyList<Product>>> GetProductsByAgentId(AgentId agentId, CancellationToken cancellationToken);
}
