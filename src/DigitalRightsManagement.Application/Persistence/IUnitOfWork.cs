namespace DigitalRightsManagement.Application.Persistence;

public interface IUnitOfWork
{
    Task SaveEntities(CancellationToken cancellationToken);
}
