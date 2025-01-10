using Bogus;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.MigrationService.Factories;

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
}
