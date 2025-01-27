using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application;

public interface ICurrentAgentProvider
{
    Task<Result<Agent>> Get(CancellationToken ct);
}
