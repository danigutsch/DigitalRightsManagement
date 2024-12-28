using DigitalRightsManagement.Common;
using Ardalis.Result;

namespace DigitalRightsManagement.Application;

public class ChangeEmailCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result> Handle(ChangeEmailCommand command, CancellationToken ct)
    {
        return await userRepository.GetById(command.UserId, ct)
            .BindAsync(user => user.ChangeEmail(command.NewEmail))
            .Tap(() => unitOfWork.SaveChanges(ct));
    }
}

public sealed record ChangeEmailCommand(Guid UserId, string NewEmail);
