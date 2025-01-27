using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Domain;
using DigitalRightsManagement.Domain.AgentAggregate;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DigitalRightsManagement.Infrastructure.Identity;

internal sealed class CurrentAgentProvider(IHttpContextAccessor httpContextAccessor, IAgentRepository agentRepository) : ICurrentAgentProvider
{
    public async Task<Result<Agent>> Get(CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        var agentIdString = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(agentIdString))
        {
            return Errors.Identity.InvalidAuthCredentials();
        }

        if (!Guid.TryParse(agentIdString, out var agentId))
        {
            return Errors.Identity.InvalidClaim();
        }

        var agent = await agentRepository.GetById(agentId, ct);

        return agent;
    }
}
