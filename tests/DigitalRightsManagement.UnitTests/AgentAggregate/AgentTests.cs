using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.AgentAggregate.Events;
using DigitalRightsManagement.Tests.Shared.Factories;
using DigitalRightsManagement.Tests.Shared.TestData;
using DigitalRightsManagement.UnitTests.Helpers.Abstractions;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.AgentAggregate;

public sealed class AgentTests : UnitTestBase
{
    private readonly Agent _agent = AgentFactory.Create();

    [Fact]
    public void Cannot_Create_With_Empty_Id()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act & Assert
        Should.Throw<ArgumentException>(() => AgentId.From(emptyId));
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Username(string emptyUsername)
    {
        // Arrange
        // Act
        var result = Agent.Create(emptyUsername, _agent.Email, _agent.Role);

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
        var result = Agent.Create(_agent.Username, invalidEmail, _agent.Role);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("email");
    }

    [Fact]
    public void Cannot_Create_With_Invalid_Role()
    {
        // Arrange
        const AgentRoles invalidRole = (AgentRoles)999;

        // Act
        var result = Agent.Create(_agent.Username, _agent.Email, invalidRole);

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
        var result = Agent.Create(_agent.Username, _agent.Email, _agent.Role);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Username.ShouldBe(_agent.Username);
        result.Value.Email.ShouldBe(_agent.Email);
        result.Value.Role.ShouldBe(_agent.Role);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = Agent.Create(_agent.Username, _agent.Email, _agent.Role);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.DomainEvents.ShouldHaveSingleItem()
            .ShouldBeOfType<AgentCreated>();
    }

    [Theory, ClassData(typeof(InvalidEmailTestData))]
    public void Cannot_Update_With_Invalid_Email(string invalidEmail)
    {
        // Arrange
        var agent = AgentFactory.Create();

        // Act
        var result = agent.ChangeEmail(invalidEmail);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("email");
    }

    [Fact]
    public void Can_Update_Email()
    {
        // Arrange
        var agent = AgentFactory.Create();

        // Act
        var result = agent.ChangeEmail(_agent.Email);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.Email.ShouldBe(_agent.Email);
    }

    [Fact]
    public void Update_Queues_Event()
    {
        // Arrange
        var agent = AgentFactory.Create();

        // Act
        var result = agent.ChangeEmail(_agent.Email);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.DomainEvents.OfType<EmailUpdated>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Admin_Can_Promote()
    {
        // Arrange
        var admin = AgentFactory.Create(role: AgentRoles.Admin);

        const AgentRoles targetRole = AgentRoles.Manager;
        var agent = AgentFactory.Create(role: AgentRoles.Worker);

        // Act
        var result = agent.ChangeRole(admin, targetRole);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.Role.ShouldBe(targetRole);
    }

    [Fact]
    public void Admin_Can_Demote()
    {
        // Arrange
        var admin = AgentFactory.Create(role: AgentRoles.Admin);

        const AgentRoles targetRole = AgentRoles.Worker;
        var agent = AgentFactory.Create(role: AgentRoles.Manager);

        // Act
        var result = agent.ChangeRole(admin, targetRole);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.Role.ShouldBe(targetRole);
    }

    [Fact]
    public void Role_Change_Queues_event()
    {
        // Arrange
        var admin = AgentFactory.Create(role: AgentRoles.Admin);
        var agent = AgentFactory.Create(role: AgentRoles.Worker);

        // Act
        var result = agent.ChangeRole(admin, AgentRoles.Manager);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.DomainEvents.OfType<AgentPromoted>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Non_Admin_Cannot_Change_Role()
    {
        // Arrange
        var promotersAndPromotees = Enum.GetValues<AgentRoles>()
            .Where(role => role != AgentRoles.Admin)
            .Select(role =>
                (
                    Promoter: AgentFactory.Create(role: role),
                    Promotee: AgentFactory.Create(role: AgentRoles.Worker)
                )
            );

        // Act
        var results = promotersAndPromotees
            .Select(tuple => tuple.Promotee.ChangeRole(tuple.Promoter, AgentRoles.Manager));

        // Assert
        results.ShouldAllBe(r => r.IsUnauthorized());
    }

    [Fact]
    public void Cannot_Change_To_Same_Role()
    {
        // Arrange
        var admin = AgentFactory.Create(role: AgentRoles.Admin);
        var agent = AgentFactory.Create(role: AgentRoles.Manager);

        // Act
        var result = agent.ChangeRole(admin, AgentRoles.Manager);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Cannot_Change_To_Unknown_Role()
    {
        // Arrange
        var admin = AgentFactory.Create(role: AgentRoles.Admin);
        var agent = AgentFactory.Create(role: AgentRoles.Manager);

        // Act
        var result = agent.ChangeRole(admin, (AgentRoles)999);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Add_Product_To_Manager()
    {
        // Arrange
        var agent = AgentFactory.Create(role: AgentRoles.Manager);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = agent.AddProduct(product.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        agent.Products.ShouldHaveSingleItem()
            .ShouldBe(product.Id);
    }

    [Fact]
    public void Cannot_Add_Product_To_Admin()
    {
        // Arrange
        var agent = AgentFactory.Create(role: AgentRoles.Admin);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = agent.AddProduct(product.Id);

        // Assert
        result.IsUnauthorized().ShouldBeTrue();
    }

    [Fact]
    public void Cannot_Add_Same_Product_Twice()
    {
        // Arrange
        var agent = AgentFactory.Create(role: AgentRoles.Manager);
        var product = ProductFactory.InDevelopment();

        // Act
        var result = agent.AddProduct(product.Id);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Act
        result = agent.AddProduct(product.Id);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }
}
