using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate.Events;
using DigitalRightsManagement.UnitTests.Tools;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.UserAggregate;

public sealed class UserTests
{
    private readonly User _validUser = UserFactory.CreateValidUser();

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Username(string emptyUsername)
    {
        // Arrange
        // Act
        var result = User.Create(emptyUsername, _validUser.Email, _validUser.Role);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("username");
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    [ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        // Act
        var result = User.Create(_validUser.Username, invalidEmail, _validUser.Role);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("email");
    }

    [Fact]
    public void Cannot_Create_With_Invalid_Role()
    {
        // Arrange
        const UserRoles invalidRole = (UserRoles)999;

        // Act
        var result = User.Create(_validUser.Username, _validUser.Email, invalidRole);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("role");
    }

    [Fact]
    public void Can_Create_User_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = User.Create(_validUser.Username, _validUser.Email, _validUser.Role);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be(_validUser.Username);
        result.Value.Email.Should().Be(_validUser.Email);
        result.Value.Role.Should().Be(_validUser.Role);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = User.Create(_validUser.Username, _validUser.Email, _validUser.Role);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreated>();
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    public void Cannot_Update_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var user = UserFactory.CreateValidUser();

        // Act
        var result = user.UpdateEmail(invalidEmail);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("email");
    }

    [Fact]
    public void Can_Update_Email()
    {
        // Arrange
        var user = UserFactory.CreateValidUser();

        // Act
        var result = user.UpdateEmail(_validUser.Email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Email.Should().Be(_validUser.Email);
    }

    [Fact]
    public void Update_Queues_Event()
    {
        // Arrange
        var user = UserFactory.CreateValidUser();

        // Act
        var result = user.UpdateEmail(_validUser.Email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.OfType<EmailUpdated>().Should().ContainSingle();
    }
}
