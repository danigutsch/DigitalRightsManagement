using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using DigitalRightsManagement.UnitTests.Tools;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests.ProductAggregate;

public sealed class ProductTests
{
    private readonly Product _validProduct = ProductFactory.InDevelopment();

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Name(string emptyName)
    {
        // Arrange
        // Act
        var result = Product.Create(emptyName, _validProduct.Description, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("name");
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Create_With_Empty_Description(string emptyDescription)
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, emptyDescription, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
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
        result.Status.Should().Be(ResultStatus.Invalid);
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
        result.Status.Should().Be(ResultStatus.Invalid);
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
        result.Status.Should().Be(ResultStatus.Invalid);
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
        priceResult.Value.Value.Should().Be(zeroPrice);
        priceResult.Value.Currency.Should().Be(_validProduct.Price.Currency);

        // Act
        var productResult = Product.Create(_validProduct.Name, _validProduct.Description, priceResult.Value, _validProduct.CreatedBy);

        // Assert
        productResult.IsSuccess.Should().BeTrue();
        productResult.Value.Name.Should().Be(_validProduct.Name);
        productResult.Value.Description.Should().Be(_validProduct.Description);
        productResult.Value.Price.Value.Should().Be(zeroPrice);
        productResult.Value.Price.Currency.Should().Be(_validProduct.Price.Currency);
    }

    [Fact]
    public void Can_Create_Product_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.CreatedBy);

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
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.CreatedBy);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProductCreated>();
    }

    [Fact]
    public void Can_Update_Price()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(newPrice, "reason");

        // Assert
        product.Price.Should().Be(newPrice);
    }

    [Fact]
    public void Price_Update_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        product.UpdatePrice(newPrice, "reason");

        // Assert
        product.DomainEvents.OfType<PriceUpdated>().Should().ContainSingle();
    }

    [Fact]
    public void Is_In_Development_After_Creation()
    {
        // Arrange
        // Act
        var product = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, _validProduct.CreatedBy).Value;

        // Assert
        product.Status.Should().Be(ProductStatus.Development);
    }

    [Fact]
    public void Can_Publish_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(Guid.NewGuid());

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
        var result = product.Publish(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Can_Not_Publish_From_Obsolete()
    {
        // Arrange
        var product = ProductFactory.Obsolete();

        // Act
        var result = product.Publish(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Can_Not_Publish_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Publish(Guid.Empty);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Publish_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        product.Publish(Guid.NewGuid());

        // Assert
        product.DomainEvents.OfType<ProductPublished>().Should().ContainSingle();
    }

    [Fact]
    public void Can_Obsolete_From_Development()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(Guid.NewGuid());

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
        var result = product.Obsolete(Guid.NewGuid());

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
        var result = product.Obsolete(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Can_Not_Obsolete_With_Empty_User_Id()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.Obsolete(Guid.Empty);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Obsolete_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.Published();

        // Act
        product.Obsolete(Guid.NewGuid());

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
        var result = product.UpdateDescription(newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        product.Description.Should().Be(newDescription);
    }

    [Fact]
    public void Description_Update_Queues_Event()
    {
        // Arrange
        var product = ProductFactory.InDevelopment();
        const string newDescription = "new description";

        // Act
        product.UpdateDescription(newDescription);

        // Assert
        product.DomainEvents.OfType<DescriptionUpdated>().Should().ContainSingle();
    }

    [Theory, ClassData(typeof(EmptyStringTestData))]
    public void Cannot_Update_With_Empty_Description(string newDescription)
    {
        // Arrange
        var product = ProductFactory.InDevelopment();

        // Act
        var result = product.UpdateDescription(newDescription);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }
}
