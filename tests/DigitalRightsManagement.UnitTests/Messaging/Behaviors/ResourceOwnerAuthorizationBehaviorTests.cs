using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Infrastructure.Messaging.Behaviors;
using DigitalRightsManagement.UnitTests.Helpers.Mocks;
using DigitalRightsManagement.UnitTests.Helpers.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalRightsManagement.UnitTests.Messaging.Behaviors;

public sealed class ResourceOwnerAuthorizationBehaviorTests
{
    private readonly TestCurrentUserProvider _userProvider;
    private readonly TestResourceRepository _resourceRepository;
    private readonly ResourceOwnerAuthorizationBehavior<BaseRequest, Result> _behavior;

    public ResourceOwnerAuthorizationBehaviorTests()
    {
        ILogger<ResourceOwnerAuthorizationBehavior<BaseRequest, Result>> logger = NullLogger<ResourceOwnerAuthorizationBehavior<BaseRequest, Result>>.Instance;

        _userProvider = new TestCurrentUserProvider();
        _resourceRepository = new TestResourceRepository();
        _behavior = new ResourceOwnerAuthorizationBehavior<BaseRequest, Result>(_userProvider, _resourceRepository, logger);
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
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Cannot_Handle_Request_With_User_Not_Found()
    {
        // Arrange
        var request = new TestResourceRequest(Guid.NewGuid());
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
    public async Task Cannot_Handle_Request_When_Not_Resource_Owner()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var request = new TestResourceRequest(resourceId);
        var user = User.Create("test", "test@test.com", UserRoles.Viewer).Value;
        _userProvider.NextResult = Result.Success(user);
        _resourceRepository.IsOwner = false;

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsUnauthorized().Should().BeTrue();
    }

    [Fact]
    public async Task Can_Handle_Request_When_Resource_Owner()
    {
        // Arrange
        var resourceId = Guid.NewGuid();
        var request = new TestResourceRequest(resourceId);
        var user = User.Create("test", "test@test.com", UserRoles.Viewer).Value;
        _userProvider.NextResult = Result.Success(user);
        _resourceRepository.IsOwner = true;

        // Act
        var result = await _behavior.Handle(
            request,
            () => Task.FromResult(Result.Success()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
