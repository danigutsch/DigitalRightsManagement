using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate.Events;

namespace DigitalRightsManagement.Domain.UserAggregate;

public static class RoleManager
{
    public static Result<User> CreateManager(User admin, string username, string email)
    {
        if (admin.Role != UserRoles.Admin)
        {
            return Errors.User.Unauthorized.CreateManager();
        }

        var userResult = User.Create(username, email, UserRoles.Manager);
        if (!userResult.IsSuccess)
        {
            return userResult;
        }

        var manager = userResult.Value;

        admin.QueueDomainEvent(new ManagerCreated(admin.Id, manager.Id));

        return manager;
    }
}
