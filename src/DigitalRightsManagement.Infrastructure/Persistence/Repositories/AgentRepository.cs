using Ardalis.Result;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Infrastructure.Persistence.Repositories;

internal sealed class AgentRepository(ManagementDbContext context) : IAgentRepository
{
    public IUnitOfWork UnitOfWork => context;

    public void Add(Agent agent) => context.Agents.Add(agent);

    public async Task<Result<Agent>> GetById(AgentId id, CancellationToken cancellationToken)
    {
        var agent = await context.Agents.FindAsync([id], cancellationToken: cancellationToken);
        if (agent is null)
        {
            return Errors.Agents.NotFound();
        }

        return agent;
    }
}
