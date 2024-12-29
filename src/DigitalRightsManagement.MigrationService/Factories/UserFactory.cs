using Bogus;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.MigrationService.Factories;

public static class UserFactory
{
    private static readonly Faker Faker = new();

    public static User Create(
        string? username = null,
        string? email = null,
        UserRoles? role = null,
        Guid? id = null)
    {
        return User.Create(
            username ?? Faker.Person.FullName,
            email ?? Faker.Internet.Email(),
            role ?? Faker.PickRandom<UserRoles>(),
            id);
    }
}
