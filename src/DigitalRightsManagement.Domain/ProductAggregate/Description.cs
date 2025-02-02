using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public readonly record struct Description
{
    public const int MaxLength = 200;
    public const int MinLength = 10;

    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description> From(string value) =>
        Normalize(value)
            .Bind(Validate)
            .Map(s => new Description(s));

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
            errors.AddRange(Errors.Products.Description.Empty().ValidationErrors);
        }
        if (input.Length is < MinLength or > MaxLength)
        {
            errors.AddRange(Errors.Products.Description.InvalidLength(MinLength, MaxLength, input.Length).ValidationErrors);
        }
    }

    public int CompareTo(string? other) => string.CompareOrdinal(Value, other);

    public static bool operator ==(Description left, string right) => left.CompareTo(right) == 0;
    public static bool operator !=(Description left, string right) => left.CompareTo(right) == 0;
    public static bool operator ==(string left, Description right) => right.CompareTo(left) == 0;
    public static bool operator !=(string left, Description right) => right.CompareTo(left) == 0;

    public bool Equals(ProductName other) => Value == other.Value;
    public bool Equals(string? other) => Value == other;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}
