using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Application;

public interface ICurrentUserProvider
{
    Task<Result<User>> Get(CancellationToken ct);
}
