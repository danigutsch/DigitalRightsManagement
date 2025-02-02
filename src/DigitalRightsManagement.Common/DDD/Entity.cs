namespace DigitalRightsManagement.Common.DDD;

public abstract class Entity<TId> where TId : struct
{
    public TId Id { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity &&
               entity.Id.Equals(Id);
    }

    public override int GetHashCode() => HashCode.Combine(Id, typeof(TId));

    protected Entity(TId id) => Id = id;

    protected Entity() { } // Do not use
}
