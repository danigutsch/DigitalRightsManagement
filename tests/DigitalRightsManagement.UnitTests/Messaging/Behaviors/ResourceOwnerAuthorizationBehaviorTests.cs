using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.UnitTests.Helpers.Mocks;
using DigitalRightsManagement.UnitTests.Helpers.TestDoubles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.Messaging.Behaviors;

public sealed class ResourceOwnerAuthorizationBehaviorTests
{
    private readonly TestCurrentAgentProvider _agentProvider;
    private readonly TestResourceRepository _resourceRepository;
    private readonly ResourceOwnerAuthorizationBehavior<BaseRequest, Result> _behavior;

    public ResourceOwnerAuthorizationBehaviorTests()
    {
        ILogger<ResourceOwnerAuthorizationBehavior<BaseRequest, Result>> logger = NullLogger<ResourceOwnerAuthorizationBehavior<BaseRequest, Result>>.Instance;

        _agentProvider = new TestCurrentAgentProvider();
        _resourceRepository = new TestResourceRepository();
        _behavior = new ResourceOwnerAuthorizationBehavior<BaseRequest, Result>(_agentProvider, _resourceRepository, logger);
    }

    [Fact]
    public async Task Can_Handle_Request_Without_Ownership_Check()
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
    public async Task Cannot_Handle_Request_With_Agent_Not_Found()
    {
        // Arrange
        var request = new TestResourceRequest(Guid.NewGuid());
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
    public async Task Cannot_Handle_Request_When_Not_Resource_Owner()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var request = new TestResourceRequest(resourceId);
        var agent = Agent.Create("test", "test@test.com", AgentRoles.Worker).Value;
        _agentProvider.NextResult = Result.Success(agent);
        _resourceRepository.IsOwner = false;

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsUnauthorized().ShouldBeTrue();
    }

    [Fact]
    public async Task Can_Handle_Request_When_Resource_Owner()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var request = new TestResourceRequest(resourceId);
        var agent = Agent.Create("test", "test@test.com", AgentRoles.Worker).Value;
        _agentProvider.NextResult = Result.Success(agent);
        _resourceRepository.IsOwner = true;

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
