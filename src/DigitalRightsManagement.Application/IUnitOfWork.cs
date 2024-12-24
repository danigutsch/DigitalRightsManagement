using Ardalis.Result;

namespace DigitalRightsManagement.Application;

public interface IUnitOfWork
{
    Task<Result> SaveChanges(CancellationToken cancellationToken);
}
