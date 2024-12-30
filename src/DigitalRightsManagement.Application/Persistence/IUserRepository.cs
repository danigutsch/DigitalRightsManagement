using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IUserRepository
{
    Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken);
}
