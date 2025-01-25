using Ardalis.Result;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Domain.UserAggregate.Events;
using DigitalRightsManagement.Tests.Shared.Factories;
using DigitalRightsManagement.Tests.Shared.TestData;
using DigitalRightsManagement.UnitTests.Helpers.Abstractions;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.UserAggregate;

public sealed class UserTests : UnitTestBase
{
    private readonly User _user = UserFactory.Create();

    [Fact]
    public void Cannot_Create_With_Empty_Id()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act
        var result = User.Create(_user.Username, _user.Email, _user.Role, emptyId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("id");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Username(string emptyUsername)
    {
        // Arrange
        // Act
        var result = User.Create(emptyUsername, _user.Email, _user.Role);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("username");
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    [ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        // Act
        var result = User.Create(_user.Username, invalidEmail, _user.Role);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("email");
    }

    [Fact]
    public void Cannot_Create_With_Invalid_Role()
    {
        // Arrange
        const UserRoles invalidRole = (UserRoles)999;

        // Act
        var result = User.Create(_user.Username, _user.Email, invalidRole);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("role");
    }

    [Fact]
    public void Can_Create_User_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = User.Create(_user.Username, _user.Email, _user.Role);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Username.ShouldBe(_user.Username);
        result.Value.Email.ShouldBe(_user.Email);
        result.Value.Role.ShouldBe(_user.Role);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = User.Create(_user.Username, _user.Email, _user.Role);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.DomainEvents.ShouldHaveSingleItem()
            .ShouldBeOfType<UserCreated>();
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    public void Cannot_Update_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(invalidEmail);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("email");
    }

    [Fact]
    public void Can_Update_Email()
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(_user.Email);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.Email.ShouldBe(_user.Email);
    }

    [Fact]
    public void Update_Queues_Event()
    {
        // Arrange
        var user = UserFactory.Create();

        // Act
        var result = user.ChangeEmail(_user.Email);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.OfType<EmailUpdated>().ShouldHaveSingleItem();
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
        result.IsSuccess.ShouldBeTrue();
        user.Role.ShouldBe(targetRole);
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
        result.IsSuccess.ShouldBeTrue();
        user.Role.ShouldBe(targetRole);
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
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.OfType<UserPromoted>().ShouldHaveSingleItem();
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
        results.ShouldAllBe(r => r.IsUnauthorized());
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
        result.IsInvalid().ShouldBeTrue();
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
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Add_Product_To_Manager()
    {
        // Arrange
        var user = UserFactory.Create(role: UserRoles.Manager);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = user.AddProduct(product.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.Products.ShouldHaveSingleItem()
            .ShouldBe(product.Id);
    }

    [Fact]
    public void Cannot_Add_Product_To_Non_Manager()
    {
        // Arrange
        var user = UserFactory.Create(role: UserRoles.Viewer);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = user.AddProduct(product.Id);

        // Assert
        result.IsUnauthorized().ShouldBeTrue();
    }

    [Fact]
    public void Cannot_Add_Same_Product_Twice()
    {
        // Arrange
        var user = UserFactory.Create(role: UserRoles.Manager);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = user.AddProduct(product.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Act
        result = user.AddProduct(product.Id);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }
}
