using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.UnitTests.Helpers.TestDoubles;

#pragma warning disable CA1812
internal sealed class TestResource(Guid id) : AggregateRoot<Guid>(id);
#pragma warning restore CA1812
