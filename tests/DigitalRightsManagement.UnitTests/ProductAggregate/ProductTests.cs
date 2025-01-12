using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.Domain.ProductAggregate.Events;
using DigitalRightsManagement.Domain.UserAggregate;
using DigitalRightsManagement.Tests.Shared.Factories;
using DigitalRightsManagement.Tests.Shared.TestData;
using DigitalRightsManagement.UnitTests.Common.Abstractions;
using FluentAssertions;

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
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("id");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Name(string emptyName)
    {
        // Arrange
        // Act
        var result = Product.Create(emptyName, _validProduct.Description, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("name");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Description(string emptyDescription)
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, emptyDescription, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("description");
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
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("price");
    }

    [Fact]
    public void Cannot_Create_With_Unknown_Currency()
    {
        // Arrange
        const Currency unknownCurrency = (Currency)999;

        // Act
        var result = Price.Create(1.00m, unknownCurrency);

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("price");
    }

    [Fact]
    public void Can_Not_Create_With_Empty_Creator_Id()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, emptyGuid);

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("created-by");
    }

    [Fact]
    public void Can_Create_Product_With_Zero_Price()
    {
        // Arrange
        const decimal zeroPrice = 0m;

        // Act
        var priceResult = Price.Create(zeroPrice, _validProduct.Price.Currency);

        // Assert
        priceResult.IsSuccess.Should().BeTrue();
        priceResult.Value.Amount.Should().Be(zeroPrice);
        priceResult.Value.Currency.Should().Be(_validProduct.Price.Currency);

        // Act
        var productResult = Product.Create(_validProduct.Name, _validProduct.Description, priceResult.Value, _validProduct.Manager);

        // Assert
        productResult.IsSuccess.Should().BeTrue();
        productResult.Value.Name.Should().Be(_validProduct.Name);
        productResult.Value.Description.Should().Be(_validProduct.Description);
        productResult.Value.Price.Amount.Should().Be(zeroPrice);
        productResult.Value.Price.Currency.Should().Be(_validProduct.Price.Currency);
    }

    [Fact]
    public void Can_Create_Product_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(_validProduct.Name);
        result.Value.Description.Should().Be(_validProduct.Description);
        result.Value.Price.Should().Be(_validProduct.Price);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProductCreated>();
    }

    [Fact]
    public void Can_Update_Price()
    {
        // Arrange
        var manager = UserFactory.Create(role: UserRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: manager.Id);
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(manager.Id, newPrice, "reason");

        // Assert
        product.Price.Should().Be(newPrice);
    }

    [Fact]
    public void Can_Not_Update_Price_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        var result = product.UpdatePrice(Guid.Empty, newPrice, "reason");

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Not_Update_Without_Owner_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;
        var randomUserId = Guid.NewGuid();

        // Act
        var result = product.UpdatePrice(randomUserId, newPrice, "reason");

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Price_Update_Queues_Event()
    {
        // Arrange
        var manager = UserFactory.Create(role: UserRoles.Manager);
        var product = ProductFactory.InDevelopment(manager: manager.Id);
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(manager.Id, newPrice, "reason");

        // Assert
        product.DomainEvents.OfType<PriceUpdated>().Should().ContainSingle();
    }

    [Fact]
    public void Is_In_Development_After_Creation()
    {
        // Arrange
        // Act
        var product = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.Manager).Value;

        // Assert
        product.Status.Should().Be(ProductStatus.Development);
    }

    [Fact]
    public void Can_Publish_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(product.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Published);
    }

    [Fact]
    public void Can_Not_Publish_From_Published()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        var result = product.Publish(product.Manager);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Not_Publish_From_Obsolete()
    {
        // Arrange
        var product = ProductFactory.Obsolete();

        // Act
        var result = product.Publish(product.Manager);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Not_Publish_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(Guid.Empty);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Publish_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        product.Publish(product.Manager);

        // Assert
        product.DomainEvents.OfType<ProductPublished>().Should().ContainSingle();
    }

    [Fact]
    public void Can_Obsolete_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(product.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Obsolete_From_Published()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        var result = product.Obsolete(product.Manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Status.Should().Be(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Not_Obsolete_From_Obsolete()
    {
        // Arrange
        var product = ProductFactory.Obsolete();

        // Act
        var result = product.Obsolete(product.Manager);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Not_Obsolete_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(Guid.Empty);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Obsolete_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        product.Obsolete(product.Manager);

        // Assert
        product.DomainEvents.OfType<ProductObsoleted>().Should().ContainSingle();
    }

    [Fact]
    public void Can_Update_Description()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";

        // Act
        var result = product.UpdateDescription(product.Manager, newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Description.Should().Be(newDescription);
    }

    [Fact]
    public void Only_Owner_Can_Update_Description()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var randomUserId = Guid.NewGuid();

        // Act
        var result = product.UpdateDescription(randomUserId, string.Empty);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Can_Not_Update_Description_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";
        var emptyUserId = Guid.Empty;

        // Act
        var result = product.UpdateDescription(emptyUserId, newDescription);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }

    [Fact]
    public void Description_Update_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";

        // Act
        product.UpdateDescription(product.Manager, newDescription);

        // Assert
        product.DomainEvents.OfType<DescriptionUpdated>().Should().ContainSingle();
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Update_With_Empty_Description(string newDescription)
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.UpdateDescription(product.Manager, newDescription);

        // Assert
        result.IsInvalid().Should().BeTrue();
    }
}
