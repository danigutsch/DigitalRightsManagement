namespace DigitalRightsManagement.Application.Persistence;

public interface IResourceRepository
{
    Task<bool> IsResourceOwner(Guid userId, Type resourceType, Guid[] resourceIds, CancellationToken ct);
}
