using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests;

public class ProductTests
{
    private static readonly Product ValidProduct = Product.Create("name", "description", Price.Create(1m, Currency.Euro));

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
        var result = Product.Create(name, ValidProduct.Description, ValidProduct.Price);

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
        var result = Product.Create(ValidProduct.Name, description, ValidProduct.Price);

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
        var result = Price.Create(price, ValidProduct.Price.Currency);

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
    public void Can_Create_Product_With_Zero_Price()
    {
        // Arrange
        // Act
        var priceResult = Price.Create(0m, ValidProduct.Price.Currency);

        // Assert
        priceResult.IsSuccess.Should().BeTrue();
        priceResult.Value.Value.Should().Be(0m);
        priceResult.Value.Currency.Should().Be(ValidProduct.Price.Currency);

        // Act
        var productResult = Product.Create(ValidProduct.Name, ValidProduct.Description, priceResult.Value);

        // Assert
        productResult.IsSuccess.Should().BeTrue();
        productResult.Value.Name.Should().Be(ValidProduct.Name);
        productResult.Value.Description.Should().Be(ValidProduct.Description);
        productResult.Value.Price.Value.Should().Be(0m);
        productResult.Value.Price.Currency.Should().Be(ValidProduct.Price.Currency);
    }

    [Fact]
    public void Can_Create_Product_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = Product.Create(ValidProduct.Name, ValidProduct.Description, ValidProduct.Price);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(ValidProduct.Name);
        result.Value.Description.Should().Be(ValidProduct.Description);
        result.Value.Price.Should().Be(ValidProduct.Price);
    }

    [Fact]
    public void Creation_Queues_Event()
    {
        // Arrange
        // Act
        var result = Product.Create(ValidProduct.Name, ValidProduct.Description, ValidProduct.Price);

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
        ValidProduct.UpdatePrice(newPrice, "reason");

        // Assert
        ValidProduct.Price.Should().Be(newPrice);
        ValidProduct.DomainEvents.OfType<PriceUpdated>().Should().ContainSingle();
    }

    [Fact]
    public void Price_Update_Queues_Event()
    {
        // Arrange
        var newPrice = Price.Create(2m, Currency.Euro).Value;

        // Act
        ValidProduct.UpdatePrice(newPrice, "reason");

        // Assert
        ValidProduct.Price.Should().Be(newPrice);
    }
}
