using DigitalRightsManagement.Domain.UserAggregate;
using System.Collections.Frozen;

namespace DigitalRightsManagement.MigrationService;

internal static class SeedData
{
    public static IEnumerable<User> GetUsers() => Users;
    public static IEnumerable<User> GetUsers(Func<User, bool> predicate) => Users.Where(predicate);

    private static FrozenSet<User> Users =>
    [
        User.Create("admin", "admin@localhost.com", UserRoles.Admin),
        User.Create("user", "user@localhost.com", UserRoles.Viewer),
        User.Create("manager", "manager@localhost.com", UserRoles.Manager)
    ];
}
