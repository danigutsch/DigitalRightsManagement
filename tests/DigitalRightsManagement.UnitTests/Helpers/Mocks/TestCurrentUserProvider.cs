using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;

namespace DigitalRightsManagement.UnitTests.Helpers.Mocks;

internal sealed class TestCurrentUserProvider : ICurrentUserProvider
{
    public Result<User>? NextResult { get; set; }

    public Task<Result<User>> Get(CancellationToken ct) =>
        Task.FromResult(NextResult ?? Result.NotFound());
}
