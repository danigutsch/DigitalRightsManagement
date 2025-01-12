using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.Infrastructure.Identity;

public interface ICurrentUserProvider
{
    Task<Result<User>> Get(CancellationToken ct);
}
