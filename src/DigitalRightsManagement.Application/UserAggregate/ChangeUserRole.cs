using Ardalis.Result;
using DigitalRightsManagement.Application.Messaging;
using DigitalRightsManagement.Application.Persistence;
using DigitalRightsManagement.Common.DDD;
using DigitalRightsManagement.Common.Messaging;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.UserAggregate;

public sealed record ChangeUserRoleCommand(Guid TargetId, UserRoles DesiredRole) : ICommand
{
    internal sealed class ChangeUserRoleCommandHandler(ICurrentUserProvider currentUserProvider, IUserRepository userRepository) : ICommandHandler<ChangeUserRoleCommand>
    {
        public async Task<Result> Handle(ChangeUserRoleCommand command, CancellationToken cancellationToken)
        {
            var currentUserResult = await currentUserProvider.Get(cancellationToken);
            if (!currentUserResult.IsSuccess)
            {
                return currentUserResult.Map();
            }

            var currentUser = currentUserResult.Value;

            return await userRepository.GetById(command.TargetId, cancellationToken)
                .BindAsync(user => user.ChangeRole(currentUser, command.DesiredRole))
                .Tap(() => userRepository.UnitOfWork.SaveEntities(cancellationToken));
        }
    }
}
