using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application.Persistence;

public interface IUserRepository
{
    void Add(User user);
    Task<Result<User>> GetById(Guid id, CancellationToken cancellationToken);
}
