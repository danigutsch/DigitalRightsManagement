using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests;

public class ProductTests
{
    private readonly Product _validProduct = Product.Create(
        "name",
        "description",
        Price.Create(1m, Currency.Euro),
        Guid.NewGuid());

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void Cannot_Create_With_Empty_Name(string name)
    {
        // Arrange
        // Act
        var result = Product.Create(name, _validProduct.Description, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void Cannot_Create_With_Empty_Description(string description)
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, description, _validProduct.Price, Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("description");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1.00)]
    public void Cannot_Create_With_Negative_Price(decimal price)
    {
        // Arrange
        // Act
        var result = Price.Create(price, _validProduct.Price.Currency);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("price");
    }

    [Fact]
    public void Cannot_Create_With_Unknown_Currency()
    {
        // Arrange
        // Act
        var result = Price.Create(1.00m, (Currency)999);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("price");
    }

    [Fact]
    public void Can_Not_Create_With_Empty_Creator_Id()
    {
        // Arrange
        // Act
        var result = Product.Create(_validProduct.Name, _validProduct.Description, _validProduct.Price, Guid.Empty);

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("created-by");
    }

    [Fact]
    public void Can_Create_Product_With_Zero_Price()
    {
        // Arrange
        // Act
        var priceResult = Price.Create(0m, _validProduct.Price.Currency);

        // Assert
        priceResult.IsSuccess.Should().BeTrue();
        priceResult.Value.Value.Should().Be(0m);
        priceResult.Value.Currency.Should().Be(_validProduct.Price.Currency);

        // Act
        var productResult = Product.Create(_validProduct.Name, _validProduct.Description, priceResult.Value, _validProduct.CreatedBy);

        // Assert
        productResult.IsSuccess.Should().BeTrue();
        productResult.Value.Name.Should().Be(_validProduct.Name);
        productResult.Value.Description.Should().Be(_validProduct.Description);
        productResult.Value.Price.Value.Should().Be(0m);
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
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        _validProduct.UpdatePrice(newPrice, "reason");

        // Assert
        _validProduct.Price.Should().Be(newPrice);
    }

    [Fact]
    public void Price_Update_Queues_Event()
    {
        // Arrange
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        _validProduct.UpdatePrice(newPrice, "reason");

        // Assert
        _validProduct.DomainEvents.OfType<PriceUpdated>().Should().ContainSingle();
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
        // Act
        var result = _validProduct.Publish(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        _validProduct.Status.Should().Be(ProductStatus.Published);
    }

    [Fact]
    public void Can_Not_Publish_From_Published()
    {
        // Arrange
        _validProduct.Publish(Guid.NewGuid());

        // Act
        var result = _validProduct.Publish(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Can_Not_Publish_From_Obsolete()
    {
        // Arrange
        _validProduct.Obsolete(Guid.NewGuid());

        // Act
        var result = _validProduct.Publish(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Publish_Queues_Event()
    {
        // Arrange
        // Act
        _validProduct.Publish(Guid.NewGuid());

        // Assert
        _validProduct.DomainEvents.OfType<ProductPublished>().Should().ContainSingle();
    }

    [Fact]
    public void Can_Obsolete_From_Development()
    {
        // Arrange
        // Act
        var result = _validProduct.Obsolete(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        _validProduct.Status.Should().Be(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Obsolete_From_Published()
    {
        // Arrange
        _validProduct.Publish(Guid.NewGuid());

        // Act
        var result = _validProduct.Obsolete(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        _validProduct.Status.Should().Be(ProductStatus.Obsolete);
    }

    [Fact]
    public void Can_Not_Obsolete_From_Obsolete()
    {
        // Arrange
        _validProduct.Obsolete(Guid.NewGuid());

        // Act
        var result = _validProduct.Obsolete(Guid.NewGuid());

        // Assert
        result.Status.Should().Be(ResultStatus.Invalid);
    }

    [Fact]
    public void Obsolete_Queues_Event()
    {
        // Arrange
        // Act
        _validProduct.Obsolete(Guid.NewGuid());

        // Assert
        _validProduct.DomainEvents.OfType<ProductObsoleted>().Should().ContainSingle();
    }
}
