using Ardalis.Result;

namespace DigitalRightsManagement.Domain.AgentAggregate;

public readonly record struct EmailAddress
{
    public const int MaxLength = 100;

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static Result<EmailAddress> From(string value) =>
        Normalize(value)
            .Bind(Validate)
            .Map(s => new EmailAddress(s));

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
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        if (input.Length > MaxLength)
        {
            errors.AddRange(Errors.Agents.EmailTooLong(MaxLength).ValidationErrors);
        }

        var trimmedEmail = input.Trim();

        if (trimmedEmail.AsSpan().ContainsAny([' ', '\n', '\r', '\t']))
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        var indexOfAt = trimmedEmail.IndexOf('@', StringComparison.Ordinal);
        if (indexOfAt <= 0)
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        if (indexOfAt != trimmedEmail.LastIndexOf('@'))
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        var indexOfDot = trimmedEmail.LastIndexOf('.');
        if (indexOfDot <= 0)
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        if (indexOfDot <= indexOfAt + 2)
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }

        if (indexOfDot == trimmedEmail.Length - 1)
        {
            errors.AddRange(Errors.Agents.InvalidEmail().ValidationErrors);
        }
    }

    public int CompareTo(string? other) => string.Compare(Value, other, StringComparison.OrdinalIgnoreCase);

    public static bool operator ==(EmailAddress left, string right) => left.CompareTo(right) == 0;
    public static bool operator !=(EmailAddress left, string right) => left.CompareTo(right) == 0;
    public static bool operator ==(string left, EmailAddress right) => right.CompareTo(left) == 0;
    public static bool operator !=(string left, EmailAddress right) => right.CompareTo(left) == 0;

    public bool Equals(EmailAddress other) => Value == other.Value;
    public bool Equals(string? other) => Value == other;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;
}
