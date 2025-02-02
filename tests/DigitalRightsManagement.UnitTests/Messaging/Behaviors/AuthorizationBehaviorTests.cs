using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.UnitTests.Helpers.Mocks;
using DigitalRightsManagement.UnitTests.Helpers.TestDoubles;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.Messaging.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly TestCurrentAgentProvider _agentProvider;
    private readonly AuthorizationBehavior<BaseRequest, Result> _behavior;

    public AuthorizationBehaviorTests()
    {
        var logger = NullLogger<AuthorizationBehavior<BaseRequest, Result>>.Instance;

        _agentProvider = new TestCurrentAgentProvider();
        _behavior = new AuthorizationBehavior<BaseRequest, Result>(_agentProvider, logger);
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
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_AuthorizeWithoutRole_AgentFound_ReturnsSuccess()
    {
        // Arrange
        var request = new AuthorizedRequestWithoutRole();
        var agent = Agent.Create(Username.From("testUsername"), EmailAddress.From("test@test.com"), AgentRoles.Worker).Value;
        _agentProvider.NextResult = Result.Success(agent);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_AgentNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new RequestWithAuthorizeAttribute();
        _agentProvider.NextResult = Result.NotFound();

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsNotFound().ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_InsufficientRole_ReturnsUnauthorized()
    {
        // Arrange
        var request = new RequestWithAuthorizeAttribute();
        var agent = Agent.Create(Username.From("testUsername"), EmailAddress.From("test@test.com"), AgentRoles.Worker).Value;
        _agentProvider.NextResult = Result.Success(agent);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsUnauthorized().ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_SufficientRole_ReturnsSuccess()
    {
        // Arrange
        var request = new RequestWithAuthorizeAttribute();
        var agent = Agent.Create(Username.From("testUsername"), EmailAddress.From("test@test.com"), AgentRoles.Manager).Value;
        _agentProvider.NextResult = Result.Success(agent);

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
