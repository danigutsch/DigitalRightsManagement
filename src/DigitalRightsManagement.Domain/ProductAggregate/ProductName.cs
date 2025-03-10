﻿using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public readonly record struct ProductName : IEquatable<string>
{
    public const int MaxLength = 50;
    public const int MinLength = 5;

    public string Value { get; }

    private ProductName(string value) => Value = value;

    public static Result<ProductName> From(string value) =>
        Normalize(value)
            .Bind(Validate)
            .Map(s => new ProductName(s));

    private static Result<string> Normalize(string input) => input.Trim();

    private static Result<string> Validate(string value)
    {
        List<ValidationError> errors = [];

        Validate(value, errors);

        return errors.Count == 0
            ? value
            : Result.Invalid(errors);
    }

    private static void Validate(string input, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            errors.AddRange(Errors.Products.Name.Empty().ValidationErrors);
        }

        if (input.Length is < MinLength or > MaxLength)
        {
            errors.AddRange(Errors.Products.Name.InvalidLength(MinLength, MaxLength, input.Length).ValidationErrors);
        }
    }

    public int CompareTo(string? other) => string.CompareOrdinal(Value, other);

    public static bool operator ==(ProductName left, string right) => left.CompareTo(right) == 0;
    public static bool operator !=(ProductName left, string right) => left.CompareTo(right) == 0;
    public static bool operator ==(string left, ProductName right) => right.CompareTo(left) == 0;
    public static bool operator !=(string left, ProductName right) => right.CompareTo(left) == 0;

    public bool Equals(ProductName other) => Value == other.Value;
    public bool Equals(string? other) => Value == other;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}
