using Ardalis.Result;
using DigitalRightsManagement.Application.Authorization;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.AgentAggregate;

namespace DigitalRightsManagement.Application.AgentAggregate;

[Authorize]
public sealed record ChangeEmailCommand(string NewEmail) : ICommand
{
    internal sealed class ChangeEmailCommandHandler(ICurrentAgentProvider currentAgentProvider, IAgentRepository agentRepository) : ICommandHandler<ChangeEmailCommand>
    {
        public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
        {
            var emailResult = EmailAddress.From(command.NewEmail);
            if (!emailResult.TryGetValue(out var newEmail))
            {
                return emailResult.Map();
            }

            return await currentAgentProvider.Get(cancellationToken)
                .BindAsync(agent => agent.ChangeEmail(newEmail))
                .Tap(() => agentRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
