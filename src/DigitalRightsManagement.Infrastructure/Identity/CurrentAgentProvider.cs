using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Persistence;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal sealed class CurrentAgentProvider(IHttpContextAccessor httpContextAccessor, IAgentRepository agentRepository) : ICurrentAgentProvider
{
    public async Task<Result<Agent>> Get(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        var agentIdString = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(agentIdString))
        {
            return Result.Invalid();
        }

        if (!Guid.TryParse(agentIdString, out var agentId))
        {
            return Result.Invalid();
        }

        var agent = await agentRepository.GetById(agentId, ct);

        return agent;
    }
}
