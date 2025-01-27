using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.AgentAggregate;

[Authorize]
public sealed record ChangeEmailCommand(string NewEmail) : ICommand
{
    internal sealed class ChangeEmailCommandHandler(ICurrentAgentProvider currentAgentProvider, IAgentRepository agentRepository) : ICommandHandler<ChangeEmailCommand>
    {
        public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
        {
            return await currentAgentProvider.Get(cancellationToken)
                .BindAsync(agent => agent.ChangeEmail(command.NewEmail))
                .Tap(() => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
