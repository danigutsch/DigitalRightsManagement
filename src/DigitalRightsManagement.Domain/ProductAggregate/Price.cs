using Ardalis.Result;
using DigitalRightsManagement.Common.DDD;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Price : ValueObject
{
    public decimal Amount { get; private init; }
    public Currency Currency { get; private init; }

    private Price(decimal amount, Currency currency)
    {
        Amount = amount;
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
        yield return Amount;
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
