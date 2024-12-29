using Bogus;

namespace DigitalRightsManagement.UnitTests.Common.Abstractions;

public abstract class UnitTestBase
{
    protected readonly Faker Faker = new();
}
