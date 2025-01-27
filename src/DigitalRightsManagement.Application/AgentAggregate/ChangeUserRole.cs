using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application.AgentAggregate;

[Authorize(AgentRoles.Manager)]
public sealed record ChangeAgentRoleCommand(Guid TargetId, AgentRoles DesiredRole) : ICommand
{
    internal sealed class ChangeAgentRoleCommandHandler(ICurrentAgentProvider currentAgentProvider, IAgentRepository agentRepository) : ICommandHandler<ChangeAgentRoleCommand>
    {
        public async Task<Result> Handle(ChangeAgentRoleCommand command, CancellationToken cancellationToken)
        {
            var currentAgentResult = await currentAgentProvider.Get(cancellationToken);
            if (!currentAgentResult.IsSuccess)
            {
                return currentAgentResult.Map();
            }

            var currentAgent = currentAgentResult.Value;

            return await agentRepository.GetById(command.TargetId, cancellationToken)
                .BindAsync(agent => agent.ChangeRole(currentAgent, command.DesiredRole))
                .Tap(() => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
