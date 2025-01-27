using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.UnitTests.Helpers.Mocks;

internal sealed class TestCurrentAgentProvider : ICurrentAgentProvider
{
    public Result<Agent>? NextResult { get; set; }

    public Task<Result<Agent>> Get(CancellationToken ct) =>
        Task.FromResult(NextResult ?? Result.NotFound());
}
