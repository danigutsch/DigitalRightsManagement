using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.AgentAggregate;

[Authorize]
public sealed record GetCurrentAgentInformationQuery : IQuery<AgentDto>
{
    internal sealed class GetCurrentAgentQueryHandler(ICurrentAgentProvider currentAgentProvider) : IQueryHandler<GetCurrentAgentInformationQuery, AgentDto>
    {
        public async Task<Result<AgentDto>> Handle(GetCurrentAgentInformationQuery request, CancellationToken cancellationToken)
        {
            return await currentAgentProvider.Get(cancellationToken)
                .MapAsync(agent => new AgentDto(
                    agent.Id.Value,
                    agent.Username,
                    agent.Email.Value,
                    agent.Role,
                    [.. agent.Products.Select(p => p.Value)]));
        }
    }
}
