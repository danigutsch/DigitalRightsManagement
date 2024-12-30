namespace DigitalRightsManagement.Common.DDD;

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj) =>
        obj?.GetType() == GetType() &&
        ((ValueObject)obj).GetEqualityComponents().SequenceEqual(GetEqualityComponents());

    public override int GetHashCode() => HashCode.Combine(GetEqualityComponents());
}
