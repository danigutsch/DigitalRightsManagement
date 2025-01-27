using Ardalis.Result;
using DigitalRightsManagement.Domain.AgentAggregate;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate.Events;
using DigitalRightsManagement.Tests.Shared.Factories;
using DigitalRightsManagement.Tests.Shared.TestData;
using DigitalRightsManagement.UnitTests.Helpers.Abstractions;
using Shouldly;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

public sealed class ProductTests : UnitTestBase
{
    private readonly Product _validProduct = ProductFactory.InDevelopment();

    [Fact]
    public void Cannot_Create_With_Empty_Id()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, Guid.NewGuid(), emptyId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("id");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Name(string emptyName)
    {
        // Arrange
        // Act
        var result = Product.Create(emptyName, _validProduct.Description, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem().ErrorCode.ShouldContain("name");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Description(string emptyDescription)
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, emptyDescription, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem().ErrorCode.ShouldContain("description");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1.00)]
    public void Cannot_Create_With_Negative_Price(decimal negativePrice)
    {
        // Arrange
        // Act
        var result = Price.Create(negativePrice, _validProduct.Price.Currency);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("price");
    }

    [Fact]
    public void Cannot_Create_With_Unknown_Currency()
    {
        // Arrange
        const Currency unknownCurrency = (Currency)999;

        // Act
        var result = Price.Create(1.00m, unknownCurrency);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("price");
    }

    [Fact]
    public void Can_Not_Create_With_Empty_Creator_Id()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, emptyGuid);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("created-by");
    }

    [Fact]
    public void Can_Create_Product_With_Zero_Price()
    {
        // Arrange
        const decimal zeroPrice = 0m;

        // Act
        var priceResult = Price.Create(zeroPrice, _validProduct.Price.Currency);

        // Assert
        priceResult.IsSuccess.ShouldBeTrue();
        priceResult.Value.Amount.ShouldBe(zeroPrice);
        priceResult.Value.Currency.ShouldBe(_validProduct.Price.Currency);

        // Act
        var productResult = Product.Create(_validProduct.Name, _validProduct.Description, priceResult.Value, _validProduct.AgentId);

        // Assert
        productResult.IsSuccess.ShouldBeTrue();
        productResult.Value.Name.ShouldBe(_validProduct.Name);
        productResult.Value.Description.ShouldBe(_validProduct.Description);
        productResult.Value.Price.Amount.ShouldBe(zeroPrice);
        productResult.Value.Price.Currency.ShouldBe(_validProduct.Price.Currency);
    }

    [Fact]
    public void Can_Create_Product_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.AgentId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe(_validProduct.Name);
        result.Value.Description.ShouldBe(_validProduct.Description);
        result.Value.Price.ShouldBe(_validProduct.Price);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.AgentId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<ProductCreated>();
    }

    [Fact]
    public void Can_Update_Price()
    {
        // Arrange
        var manager = AgentFactory.Create(role: AgentRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: manager.Id);
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(manager.Id, newPrice, "reason");

        // Assert
        product.Price.ShouldBe(newPrice);
    }

    [Fact]
    public void Can_Not_Update_Price_With_Empty_Agent_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        var result = product.UpdatePrice(Guid.Empty, newPrice, "reason");

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Not_Update_Without_Owner_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;
        var randomAgentId = Guid.NewGuid();

        // Act
        var result = product.UpdatePrice(randomAgentId, newPrice, "reason");

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Price_Update_Queues_Event()
    {
        // Arrange
        var manager = AgentFactory.Create(role: AgentRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: manager.Id);
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(manager.Id, newPrice, "reason");

        // Assert
        product.DomainEvents.OfType<PriceUpdated>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Is_In_Development_After_Creation()
    {
        // Arrange
        // Act
        var product = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.AgentId).Value;

        // Assert
        product.Status.ShouldBe(ProductStatus.Development);
    }

    [Fact]
    public void Can_Publish_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(product.AgentId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Status.ShouldBe(ProductStatus.Published);
    }

    [Fact]
    public void Can_Not_Publish_From_Published()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        var result = product.Publish(product.AgentId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Not_Publish_From_Obsolete()
    {
        // Arrange
        var product = ProductFactory.Obsolete();

        // Act
        var result = product.Publish(product.AgentId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Not_Publish_With_Empty_Agent_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(Guid.Empty);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Publish_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        product.Publish(product.AgentId);

        // Assert
        product.DomainEvents.OfType<ProductPublished>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Can_Obsolete_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(product.AgentId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Status.ShouldBe(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Obsolete_From_Published()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        var result = product.Obsolete(product.AgentId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Status.ShouldBe(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Not_Obsolete_From_Obsolete()
    {
        // Arrange
        var product = ProductFactory.Obsolete();

        // Act
        var result = product.Obsolete(product.AgentId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Not_Obsolete_With_Empty_Agent_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(Guid.Empty);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Obsolete_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        product.Obsolete(product.AgentId);

        // Assert
        product.DomainEvents.OfType<ProductObsoleted>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Can_Update_Description()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";

        // Act
        var result = product.UpdateDescription(product.AgentId, newDescription);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Description.ShouldBe(newDescription);
    }

    [Fact]
    public void Only_Owner_Can_Update_Description()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var randomAgentId = Guid.NewGuid();

        // Act
        var result = product.UpdateDescription(randomAgentId, string.Empty);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Not_Update_Description_With_Empty_Agent_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";
        var emptyAgentId = Guid.Empty;

        // Act
        var result = product.UpdateDescription(emptyAgentId, newDescription);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Description_Update_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";

        // Act
        product.UpdateDescription(product.AgentId, newDescription);

        // Assert
        product.DomainEvents.OfType<DescriptionUpdated>().ShouldHaveSingleItem();
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Update_With_Empty_Description(string newDescription)
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.UpdateDescription(product.AgentId, newDescription);

        // Assert
        result.IsInvalid().ShouldBeTrue();
    }

    [Fact]
    public void Can_Assign_Worker()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();

        // Act
        var result = product.AssignWorker(product.AgentId, workerId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.AssignedWorkers.ShouldHaveSingleItem().ShouldBe(workerId);
    }

    [Fact]
    public void Cannot_Assign_Empty_Worker_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var emptyWorkerId = Guid.Empty;

        // Act
        var result = product.AssignWorker(product.AgentId, emptyWorkerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("empty");
    }

    [Fact]
    public void Cannot_Assign_Same_Worker_Twice()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();

        // Act
        var result = product.AssignWorker(product.AgentId, workerId);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Act
        result = product.AssignWorker(product.AgentId, workerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("already-assigned");
    }

    [Fact]
    public void Only_Owner_Can_Assign_Worker()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();
        var randomUserId = Guid.NewGuid();

        // Act
        var result = product.AssignWorker(randomUserId, workerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("manager.invalid");
    }

    [Fact]
    public void Assignment_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();

        // Act
        var result = product.AssignWorker(product.AgentId, workerId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.DomainEvents.OfType<WorkerAssigned>().ShouldHaveSingleItem();
    }

    [Fact]
    public void Can_Unassign_Worker()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();
        product.AssignWorker(product.AgentId, workerId);

        // Act
        var result = product.UnassignWorker(product.AgentId, workerId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.AssignedWorkers.ShouldNotContain(workerId);
    }

    [Fact]
    public void Cannot_Unassign_Worker_Not_Assigned()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();

        // Act
        var result = product.UnassignWorker(product.AgentId, workerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("not-assigned");
    }

    [Fact]
    public void Cannot_Unassign_With_Empty_Worker_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var emptyWorkerId = Guid.Empty;

        // Act
        var result = product.UnassignWorker(product.AgentId, emptyWorkerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("empty");
    }

    [Fact]
    public void Only_Owner_Can_Unassign_Worker()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();
        var randomUserId = Guid.NewGuid();

        // Act
        var result = product.UnassignWorker(randomUserId, workerId);

        // Assert
        result.IsInvalid().ShouldBeTrue();
        result.ValidationErrors.ShouldHaveSingleItem()
            .ErrorCode.ShouldContain("manager.invalid");
    }

    [Fact]
    public void Unassignment_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var workerId = Guid.NewGuid();
        product.AssignWorker(product.AgentId, workerId);

        // Act
        var result = product.UnassignWorker(product.AgentId, workerId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.DomainEvents.OfType<WorkerUnassigned>().ShouldHaveSingleItem();
    }
}
