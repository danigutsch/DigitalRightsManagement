using Ardalis.Result;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application;

public sealed class ChangeUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result> Handle(ChangeUserRoleCommand command, CancellationToken ct)
    {
        return await userRepository.GetById(command.AdminId, ct)
            .DoubleBind(_ => userRepository.GetById(command.TargetId, ct))
            .BindAsync(t => t.Prev.ChangeRole(t.Next, command.DesiredRole))
            .Tap(_ => unitOfWork.SaveChanges(ct));
    }
}

public sealed record ChangeUserRoleCommand(Guid AdminId, Guid TargetId, UserRoles DesiredRole);
