using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.UnitTests.Common.Factories;
using DigitalRightsManagement.UnitTests.Common.TestData;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

public class RoleManagerTests
{
    private readonly User _validUser = UserFactory.CreateValidUser();
    private readonly User _validAdmin = UserFactory.CreateValidUser(role: UserRoles.Admin);

    [Fact]
    public void Admin_Can_Create_Manager()
    {
        // Arrange
        var admin = UserFactory.CreateValidUser(role: UserRoles.Admin);

        // Act
        var result = RoleManager.CreateManager(admin, _validUser.Username, _validUser.Email);

        // Assert
        result.Status.Should().Be(ResultStatus.Ok);
        result.Value.Role.Should().Be(UserRoles.Manager);
    }

    [Fact]
    public void Non_Admin_Cannot_Create_Manager()
    {
        // Arrange
        var nonAdminUsers = Enum.GetValues<UserRoles>()
            .Where(role => role != UserRoles.Admin)
            .Select(role => UserFactory.CreateValidUser(role: role));

        // Act
        var results = nonAdminUsers.Select(user => RoleManager.CreateManager(user, _validUser.Username, _validUser.Email));

        // Assert
        results.Should().AllSatisfy(r => r.IsUnauthorized());
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Can_Not_Create_Manager_With_EmptyName(string emptyName)
    {
        // Arrange
        // Act
        var result = RoleManager.CreateManager(_validAdmin, emptyName, _validUser.Email);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    public void Can_Not_Create_Manager_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        // Act
        var result = RoleManager.CreateManager(_validAdmin, _validUser.Username, invalidEmail);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }
}
