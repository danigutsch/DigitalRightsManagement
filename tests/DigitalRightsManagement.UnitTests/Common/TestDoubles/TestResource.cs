using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.UnitTests.Common.TestDoubles;

#pragma warning disable CA1812
internal sealed class TestResource(Guid id) : AggregateRoot(id);
#pragma warning restore CA1812
