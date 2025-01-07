using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;

namespace DigitalRightsManagement.Application.UserAggregate;

public class ChangeEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<ChangeEmailCommand>
{
    public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
    {
        return await userRepository.GetById(command.UserId, cancellationToken)
            .BindAsync(user => user.ChangeEmail(command.NewEmail))
            .Tap(() => unitOfWork.SaveChanges(cancellationToken));
    }
}

public sealed record ChangeEmailCommand(Guid UserId, string NewEmail) : ICommand;
