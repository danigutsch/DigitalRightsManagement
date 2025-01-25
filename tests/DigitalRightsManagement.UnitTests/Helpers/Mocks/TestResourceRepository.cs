using DigitalRightsManagement.Application.Persistence;

namespace DigitalRightsManagement.UnitTests.Helpers.Mocks;

internal sealed class TestResourceRepository : IResourceRepository
{
    public bool IsOwner { get; set; }

    public Task<bool> IsResourceOwner(Guid userId, Type resourceType, Guid[] resourceIds, CancellationToken ct)
    {
        return Task.FromResult(IsOwner);
    }
}
