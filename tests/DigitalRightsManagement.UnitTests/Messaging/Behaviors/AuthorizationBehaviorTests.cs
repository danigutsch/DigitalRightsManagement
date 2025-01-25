using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.UnitTests.Helpers.Mocks;
using DigitalRightsManagement.UnitTests.Helpers.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalRightsManagement.UnitTests.Messaging.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly TestCurrentUserProvider _userProvider;
    private readonly ILogger<AuthorizationBehavior<BaseRequest, Result>> _logger;
    private readonly AuthorizationBehavior<BaseRequest, Result> _behavior;

    public AuthorizationBehaviorTests()
    {
        _userProvider = new TestCurrentUserProvider();
        _logger = NullLogger<AuthorizationBehavior<BaseRequest, Result>>.Instance;
        _behavior = new AuthorizationBehavior<BaseRequest, Result>(_userProvider, _logger);
    }

    [Fact]
    public async Task Handle_RequestWithoutAttribute_ReturnsSuccess()
    {
        // Arrange
        var request = new RequestWithoutAuthorizeAttribute();

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
        var request = new RequestWithAuthorizeAttribute();
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
        var request = new RequestWithAuthorizeAttribute();
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
        var request = new RequestWithAuthorizeAttribute();
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
}
