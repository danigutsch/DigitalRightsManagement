using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate.Events;
using DigitalRightsManagement.MigrationService.Factories;
using DigitalRightsManagement.UnitTests.Common.Abstractions;
using DigitalRightsManagement.UnitTests.Common.TestData;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.UserAggregate;

public sealed class UserTests : UnitTestBase
{
    private readonly User _user = UserFactory.Create();

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Username(string emptyUsername)
    {
        // Arrange
        // Act
        var result = User.Create(emptyUsername, _user.Email, _user.Role);

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
        var result = User.Create(_user.Username, invalidEmail, _user.Role);

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
        var result = User.Create(_user.Username, _user.Email, invalidRole);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("role");
    }

    [Fact]
    public void Can_Create_User_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = User.Create(_user.Username, _user.Email, _user.Role);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be(_user.Username);
        result.Value.Email.Should().Be(_user.Email);
        result.Value.Role.Should().Be(_user.Role);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = User.Create(_user.Username, _user.Email, _user.Role);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreated>();
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    public void Cannot_Update_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(invalidEmail);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("email");
    }

    [Fact]
    public void Can_Update_Email()
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(_user.Email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Email.Should().Be(_user.Email);
    }

    [Fact]
    public void Update_Queues_Event()
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(_user.Email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.OfType<EmailUpdated>().Should().ContainSingle();
    }

    [Fact]
    public void Admin_Can_Promote()
    {
        // Arrange
        var admin = UserFactory.Create(role: UserRoles.Admin);

        const UserRoles targetRole = UserRoles.Manager;
        var user = UserFactory.Create(role: UserRoles.Viewer);

        // Act
        var result = user.ChangeRole(admin, targetRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(targetRole);
    }

    [Fact]
    public void Admin_Can_Demote()
    {
        // Arrange
        var admin = UserFactory.Create(role: UserRoles.Admin);

        const UserRoles targetRole = UserRoles.Viewer;
        var user = UserFactory.Create(role: UserRoles.Manager);

        // Act
        var result = user.ChangeRole(admin, targetRole);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(targetRole);
    }

    [Fact]
    public void Role_Change_Queues_event()
    {
        // Arrange
        var admin = UserFactory.Create(role: UserRoles.Admin);
        var user = UserFactory.Create(role: UserRoles.Viewer);

        // Act
        var result = user.ChangeRole(admin, UserRoles.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.OfType<UserPromoted>().Should().ContainSingle();
    }

    [Fact]
    public void Non_Admin_Cannot_Change_Role()
    {
        // Arrange
        var promotersAndPromotees = Enum.GetValues<UserRoles>()
            .Where(role => role != UserRoles.Admin)
            .Select(role =>
                (
                    Promoter: UserFactory.Create(role: role),
                    Promotee: UserFactory.Create(role: UserRoles.Viewer)
                )
            );

        // Act
        var results = promotersAndPromotees
            .Select(tuple => tuple.Promotee.ChangeRole(tuple.Promoter, UserRoles.Manager));

        // Assert
        results.Should().AllSatisfy(r => r.IsUnauthorized());
    }

    [Fact]
    public void Cannot_Change_To_Same_Role()
    {
        // Arrange
        var admin = UserFactory.Create(role: UserRoles.Admin);
        var user = UserFactory.Create(role: UserRoles.Manager);

        // Act
        var result = user.ChangeRole(admin, UserRoles.Manager);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Cannot_Change_To_Unknown_Role()
    {
        // Arrange
        var admin = UserFactory.Create(role: UserRoles.Admin);
        var user = UserFactory.Create(role: UserRoles.Manager);

        // Act
        var result = user.ChangeRole(admin, (UserRoles)999);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Add_Product_To_Manager()
    {
        // Arrange
        var user = UserFactory.Create(role: UserRoles.Manager);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = user.AddProduct(product);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Products.Should().ContainSingle().Which.Should().Be(product.Id);
    }

    [Fact]
    public void Cannot_Add_Product_To_Non_Manager()
    {
        // Arrange
        var user = UserFactory.Create(role: UserRoles.Viewer);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = user.AddProduct(product);

        // Assert
        result.IsUnauthorized().Should().BeTrue();
    }
}
