using DigitalRightsManagement.Application.Persistence;

namespace DigitalRightsManagement.UnitTests.Helpers.Mocks;

internal sealed class TestOwnershipService : IOwnershipService
{
    public bool IsOwner { get; set; }

    public Task<bool> IsResourceOwner(Guid ownerId, Type resourceType, Guid[] resourceIds, CancellationToken ct) => Task.FromResult(IsOwner);
}
