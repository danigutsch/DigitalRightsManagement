using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public sealed class Price : IEquatable<Price>
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
        var validation = Validate(value, currency);
        if (!validation.IsSuccess)
        {
            return validation;
        }

        return new Price(value, currency);
    }

    private static Result Validate(decimal value, Currency currency)
    {
        if (value < 0)
        {
            return Errors.Products.InvalidPrice(value);
        }

        if (!Enum.IsDefined(currency))
        {
            return Errors.Products.InvalidCurrency(currency);
        }

        return Result.Success();
    }

    public override string ToString() => $"{nameof(Price)} {{ {nameof(Amount)} = {Amount:N2}, {nameof(Currency)} = {Currency} }}";

    public bool Equals(Price? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return (Amount, Currency) == (other.Amount, other.Currency);
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Price other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Amount, (int)Currency);
    public static bool operator ==(Price? left, Price? right) => Equals(left, right);
    public static bool operator !=(Price? left, Price? right) => !Equals(left, right);
}

public enum Currency
{
    Euro,
    Dollar,
    BritishPound,
    Yen,
    BrazilianReal,
}
