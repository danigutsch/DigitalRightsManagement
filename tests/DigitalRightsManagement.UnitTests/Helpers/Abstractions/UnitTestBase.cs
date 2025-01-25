using Bogus;

namespace DigitalRightsManagement.UnitTests.Helpers.Abstractions;

public abstract class UnitTestBase
{
    protected Faker Faker { get; } = new();
}
