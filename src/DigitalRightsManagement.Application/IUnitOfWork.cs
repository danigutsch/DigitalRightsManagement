namespace DigitalRightsManagement.Application;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken cancellationToken);
}
