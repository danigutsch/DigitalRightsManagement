using Ardalis.Result;

namespace DigitalRightsManagement.Domain.AgentAggregate;

public readonly record struct Username
{
    public const int MaxLength = 50;
    public const int MinLength = 5;

    public string Value { get; }

    private Username(string value) => Value = value;

    public static Result<Username> From(string value) =>
        Normalize(value)
            .Bind(Validate)
            .Map(s => new Username(s));

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
            errors.AddRange(Errors.Agents.EmptyUsername().ValidationErrors);
        }

        if (input.Length > MaxLength)
        {
            errors.AddRange(Errors.Agents.InvalidUsernameLength(MinLength, MaxLength, input.Length).ValidationErrors);
        }
    }

    public int CompareTo(string? other) => string.Compare(Value, other, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(Username left, string right) => left.CompareTo(right) == 0;
    public static bool operator !=(Username left, string right) => left.CompareTo(right) == 0;
    public static bool operator ==(string left, Username right) => right.CompareTo(left) == 0;
    public static bool operator !=(string left, Username right) => right.CompareTo(left) == 0;

    public bool Equals(Username other) => Value == other.Value;
    public bool Equals(string? other) => Value == other;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}
