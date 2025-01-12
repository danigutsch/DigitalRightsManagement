using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Identity;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed class ChangeUserRoleCommandHandler(ICurrentUserProvider currentUserProvider, IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<ChangeUserRoleCommand>
{
    public async Task<Result> Handle(ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        return await currentUserProvider.Get(cancellationToken)
            .DoubleBind(_ => userRepository.GetById(command.TargetId, cancellationToken))
            .BindAsync(t => t.Next.ChangeRole(t.Prev, command.DesiredRole))
            .Tap(() => unitOfWork.SaveChanges(cancellationToken));
    }
}

public sealed record ChangeUserRoleCommand(Guid TargetId, UserRoles DesiredRole) : ICommand;
