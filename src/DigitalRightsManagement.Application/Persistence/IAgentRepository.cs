using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IAgentRepository : IRepository<Agent>
{
    void Add(Agent agent);
    Task<Result<Agent>> GetById(Guid id, CancellationToken cancellationToken);
}
