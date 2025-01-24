using Ardalis.Result;
using DigitalRightsManagement.Application;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.UnitTests.Common.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalRightsManagement.UnitTests.Messaging.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly TestCurrentUserProvider _userProvider;
    private readonly ILogger<AuthorizationBehavior<TestRequest, Result>> _logger;
    private readonly AuthorizationBehavior<TestRequest, Result> _behavior;

    public AuthorizationBehaviorTests()
    {
        _userProvider = new TestCurrentUserProvider();
        _logger = NullLogger<AuthorizationBehavior<TestRequest, Result>>.Instance;
        _behavior = new AuthorizationBehavior<TestRequest, Result>(_userProvider, _logger);
    }

    [Fact]
    public async Task Handle_RequestWithoutAttribute_ReturnsSuccess()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AuthorizeWithoutRole_UserFound_ReturnsSuccess()
    {
        // Arrange
        var request = new AuthorizedRequestWithoutRole();
        var user = User.Create("test", "test@test.com", UserRoles.Viewer).Value;
        _userProvider.NextResult = Result.Success(user);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new AuthorizedRequest();
        _userProvider.NextResult = Result.NotFound();

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsNotFound().Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InsufficientRole_ReturnsUnauthorized()
    {
        // Arrange
        var request = new AuthorizedRequest();
        var user = User.Create("test", "test@test.com", UserRoles.Viewer).Value;
        _userProvider.NextResult = Result.Success(user);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsUnauthorized().Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SufficientRole_ReturnsSuccess()
    {
        // Arrange
        var request = new AuthorizedRequest();
        var user = User.Create("test", "test@test.com", UserRoles.Manager).Value;
        _userProvider.NextResult = Result.Success(user);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private sealed class TestCurrentUserProvider : ICurrentUserProvider
    {
        public Result<User>? NextResult { get; set; }

        public Task<Result<User>> Get(CancellationToken ct) =>
            Task.FromResult(NextResult ?? Result.NotFound());
    }
}
