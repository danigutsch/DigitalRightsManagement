using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed class ChangeUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<ChangeUserRoleCommand, Result>
{
    public async Task<Result> Handle(ChangeUserRoleCommand command, CancellationToken ct)
    {
        return await userRepository.GetById(command.AdminId, ct)
            .DoubleBind(_ => userRepository.GetById(command.TargetId, ct))
            .BindAsync(t => t.Next.ChangeRole(t.Prev, command.DesiredRole))
            .Tap(() => unitOfWork.SaveChanges(ct));
    }
}

public sealed record ChangeUserRoleCommand(Guid AdminId, Guid TargetId, UserRoles DesiredRole) : ICommand<Result>;
