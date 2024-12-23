using Bogus;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.UnitTests.Common.Factories;

internal static class UserFactory
{
    private static readonly Faker Faker = new();

    public static User CreateValidUser(
        string? username = null,
        string? email = null,
        UserRoles? role = null)
    {
        return User.Create(
            username ?? Faker.Person.FullName,
            email ?? Faker.Internet.Email(),
            role ?? Faker.PickRandom<UserRoles>());
    }
}
