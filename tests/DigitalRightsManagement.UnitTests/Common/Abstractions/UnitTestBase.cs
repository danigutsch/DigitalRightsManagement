using Bogus;

namespace DigitalRightsManagement.UnitTests.Common.Abstractions;

public abstract class UnitTestBase
{
    protected Faker Faker { get; } = new();
}
