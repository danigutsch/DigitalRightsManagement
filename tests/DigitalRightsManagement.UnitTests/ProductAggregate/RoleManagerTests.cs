using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate.Events;
using DigitalRightsManagement.UnitTests.Common.Factories;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

public class RoleManagerTests
{
    [Fact]
    public void Admin_Can_Promote()
    {
        // Arrange
        var admin = UserFactory.CreateValidUser(role: UserRoles.Admin);
        var user = UserFactory.CreateValidUser(role: UserRoles.Viewer);

        // Act
        var result = user.Promote(admin, UserRoles.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(UserRoles.Manager);
    }

    [Fact]
    public void Promotion_Creation_Queues_event()
    {
        // Arrange
        var admin = UserFactory.CreateValidUser(role: UserRoles.Admin);
        var user = UserFactory.CreateValidUser(role: UserRoles.Viewer);

        // Act
        var result = user.Promote(admin, UserRoles.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.OfType<UserPromoted>().Should().ContainSingle();
    }

    [Fact]
    public void Non_Admin_Cannot_Create_Manager()
    {
        // Arrange
        var promotersAndPromotees = Enum.GetValues<UserRoles>()
            .Where(role => role != UserRoles.Admin)
            .Select(role =>
                (
                    Promoter: UserFactory.CreateValidUser(role: role),
                    Promotee: UserFactory.CreateValidUser(role: UserRoles.Viewer)
                )
            );

        // Act
        var results = promotersAndPromotees
            .Select(tuple => tuple.Promotee.Promote(tuple.Promoter, UserRoles.Manager));

        // Assert
        results.Should().AllSatisfy(r => r.IsUnauthorized());
    }

    [Fact]
    public void Cannot_Promote_To_Same_Role()
    {
        // Arrange
        var admin = UserFactory.CreateValidUser(role: UserRoles.Admin);
        var user = UserFactory.CreateValidUser(role: UserRoles.Manager);

        // Act
        var result = user.Promote(admin, UserRoles.Manager);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Cannot_Promote_To_Unknown_Role()
    {
        // Arrange
        var admin = UserFactory.CreateValidUser(role: UserRoles.Admin);
        var user = UserFactory.CreateValidUser(role: UserRoles.Manager);

        // Act
        var result = user.Promote(admin, (UserRoles)999);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }
}
