namespace DigitalRightsManagement.Application.Persistence;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken cancellationToken);
}
