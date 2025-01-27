namespace DigitalRightsManagement.Application.Persistence;

public interface IResourceRepository
{
    Task<bool> IsResourceOwner(Guid ownerId, Type resourceType, Guid[] resourceIds, CancellationToken ct);
}
