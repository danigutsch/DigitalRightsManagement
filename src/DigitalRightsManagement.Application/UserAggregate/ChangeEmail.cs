using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Infrastructure.Identity;

namespace DigitalRightsManagement.Application.UserAggregate;

public class ChangeEmailCommandHandler(ICurrentUserProvider currentUserProvider, IUserRepository userRepository) : ICommandHandler<ChangeEmailCommand>
{
    public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
    {
        return await currentUserProvider.Get(cancellationToken)
            .BindAsync(user => user.ChangeEmail(command.NewEmail))
            .Tap(() => userRepository.UnitOfWork.SaveChanges(cancellationToken));
    }
}

public sealed record ChangeEmailCommand(string NewEmail) : ICommand;
