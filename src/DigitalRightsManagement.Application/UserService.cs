using Ardalis.Result;
using DigitalRightsManagement.Common;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application;

internal class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result> ChangeUserRole(Guid adminId, Guid targetId, UserRoles desiredRole, CancellationToken cancellationToken)
    {
        return await userRepository.GetById(adminId, cancellationToken)
            .DoubleBind(_ => userRepository.GetById(targetId, cancellationToken))
            .BindAsync(t => t.Prev.ChangeRole(t.Next, desiredRole))
            .Tap(_ => unitOfWork.SaveChanges(cancellationToken));
    }
}
