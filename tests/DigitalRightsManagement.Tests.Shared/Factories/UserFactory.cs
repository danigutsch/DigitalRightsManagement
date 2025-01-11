using Bogus;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.MigrationService;

namespace DigitalRightsManagement.Tests.Shared.Factories;

public static class UserFactory
{
    private static readonly Faker<User> Faker = new Faker<User>()
        .CustomInstantiator(f => User.Create(
            f.Person.UserName,
            f.Internet.Email(),
            f.PickRandom<UserRoles>()));

    public static User Create(
        string? username = null,
        string? email = null,
        UserRoles? role = null,
        Guid? id = null)
    {
        var user = Faker.Generate();

        return User.Create(
            username ?? user.Username,
            email ?? user.Email,
            role ?? user.Role,
            id);
    }

    public static User Seeded(UserRoles? role = null)
    {
        IEnumerable<User> filtered = SeedData.Users;

        if (role is not null)
        {
            filtered = filtered.Where(u => u.Role == role);
        }

        return filtered.Random();
    }

    public static User Seeded(Func<User, bool> func) => SeedData.Users.Where(func).Random();
}
