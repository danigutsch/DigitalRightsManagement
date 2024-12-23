using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.UnitTests.Tools;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.UserAggregate;

public sealed class UserTests
{
    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Username(string username)
    {
        // Arrange
        const string validEmail = "user@example.com";
        const UserRoles validRole = UserRoles.Viewer;

        // Act
        var result = User.Create(username, validEmail, validRole);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("username");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("invalidemail@")]
    [InlineData("invalid@ email.com")]
    [InlineData("invalidemail.com")]
    [InlineData("invalid@@example.com")]
    [InlineData("@example.com")]
    [InlineData("inv alid@example.com")]
    [ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Invalid_Email(string email)
    {
        // Arrange
        const string validUsername = "ValidUser";
        const UserRoles validRole = UserRoles.Viewer;

        // Act
        var result = User.Create(validUsername, email, validRole);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("email");
    }

    [Fact]
    public void Cannot_Create_With_Invalid_Role()
    {
        // Arrange
        const string validUsername = "ValidUser";
        const string validEmail = "user@example.com";
        const UserRoles invalidRole = (UserRoles)999;

        // Act
        var result = User.Create(validUsername, validEmail, invalidRole);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("role");
    }

    [Fact]
    public void Can_Create_User_With_Valid_Arguments()
    {
        // Arrange
        const string validUsername = "ValidUser";
        const string validEmail = "user@example.com";
        const UserRoles validRole = UserRoles.Viewer;

        // Act
        var result = User.Create(validUsername, validEmail, validRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be(validUsername);
        result.Value.Email.Should().Be(validEmail);
        result.Value.Role.Should().Be(validRole);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        const string validUsername = "ValidUser";
        const string validEmail = "user@example.com";
        const UserRoles validRole = UserRoles.Viewer;

        // Act
        var result = User.Create(validUsername, validEmail, validRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreated>();
    }
}
