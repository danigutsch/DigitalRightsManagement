using Bogus;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.UnitTests.UserAggregate;

internal static class UserFactory
{
    private static readonly Faker<User> Faker = new Faker<User>()
        .CustomInstantiator(f => User.Create(
                f.Internet.UserName(),
                f.Internet.Email(),
                f.PickRandom<UserRoles>()
            ).Value
        );

    public static User CreateValidUser() => Faker.Generate();
}
