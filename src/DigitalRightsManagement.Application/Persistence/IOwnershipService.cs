namespace DigitalRightsManagement.Application.Persistence;

public interface IOwnershipService
{
    Task<bool> IsResourceOwner(Guid ownerId, Type resourceType, Guid[] resourceIds, CancellationToken ct);
}
