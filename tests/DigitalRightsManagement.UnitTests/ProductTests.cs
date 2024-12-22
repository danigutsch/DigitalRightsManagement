using Ardalis.Result;
using DigitalRightsManagement.Domain.ProductAggregate;
using FluentAssertions;

namespace DigitalRightsManagement.UnitTests;

public class ProductTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void Cannot_Create_With_Empty_Name(string? name)
    {
        // Arrange
        // Act
        var result = Product.Create(name!, "description", 1m);

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    public void Cannot_Create_With_Empty_Description(string? description)
    {
        // Arrange
        // Act
        var result = Product.Create("name", description!, 1m);

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("description");
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1.00)]
    public void Cannot_Create_With_Empty_Negative_Price(decimal price)
    {
        // Arrange
        // Act
        var result = Product.Create("name", "description", price);

        // Assert
        result.IsInvalid().Should().BeTrue();
        result.ValidationErrors.Should().ContainSingle().Which.ErrorCode.Should().Contain("price");
    }

    [Fact]
    public void Can_Create_Product_With_Zero_Price()
    {
        // Arrange
        // Act
        var result = Product.Create("name", "description", 0m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("name");
        result.Value.Description.Should().Be("description");
        result.Value.Price.Should().Be(0m);
    }

    [Fact]
    public void Can_Create_Product_With_Valid_Arguments()
    {
        // Arrange
        // Act
        var result = Product.Create("name", "description", 1m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("name");
        result.Value.Description.Should().Be("description");
        result.Value.Price.Should().Be(1m);
    }
}
