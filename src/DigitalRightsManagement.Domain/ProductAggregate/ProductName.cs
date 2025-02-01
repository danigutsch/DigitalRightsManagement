using Ardalis.Result;

namespace DigitalRightsManagement.Domain.ProductAggregate;

public readonly record struct ProductName
{
    public const int MaxLength = 100;
    public const int MinLength = 5;

    public string Value { get; }

    private ProductName(string value) => Value = value;

    public static Result<ProductName> From(string value) => Validate(value).Map(s => new ProductName(s));

    private static Result<string> Validate(string value)
    {
        List<ValidationError> errors = [];

        Validate(value, errors);

        return errors.Count == 0
            ? value
            : Result.Invalid(errors);
    }

    private static void Validate(string value, List<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.AddRange(Errors.Products.Name.Empty().ValidationErrors);
        }

        if (value.Length is < MinLength or > MaxLength)
        {
            errors.AddRange(Errors.Products.Name.InvalidLength(MinLength, MaxLength, value.Length).ValidationErrors);
        }
    }
}
