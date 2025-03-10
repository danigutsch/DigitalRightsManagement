﻿using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public readonly record struct Price
{
    public decimal Amount { get; private init; }
    public Currency Currency { get; private init; }

    private Price(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Price> From(decimal value, Currency currency)
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
            return Errors.Products.Price.Negative(value);
        }

        if (!Enum.IsDefined(currency))
        {
            return Errors.Products.Price.UnknownCurrency(currency);
        }

        return Result.Success();
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
