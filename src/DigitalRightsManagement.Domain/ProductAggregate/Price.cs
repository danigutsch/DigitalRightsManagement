using Ardalis.Result;
using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Price : ValueObject
{
    public decimal Value { get; }
    public Currency Currency { get; }

    private Price(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }

    public static Result<Price> Create(decimal value, Currency currency)
    {
        if (value < 0)
        {
            return Errors.Product.InvalidPrice(value);
        }

        if (!Enum.IsDefined(currency))
        {
            return Errors.Product.InvalidCurrency(currency);
        }

        return new Price(value, currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return Currency;
    }
}

public enum Currency
{
    Euro,
    Dollar,
    BritishPound,
    Yen,
    BrazilianReal,
}
