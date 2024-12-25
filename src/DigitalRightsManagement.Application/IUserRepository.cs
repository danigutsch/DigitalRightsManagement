using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application;

public interface IUserRepository
{
    Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken);
}
