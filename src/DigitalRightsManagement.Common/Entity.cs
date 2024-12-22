namespace DigitalRightsManagement.Common;

public abstract class Entity
{
    public Guid Id { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        return ((Entity)obj).Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    protected Entity(Guid id) => Id = id;

    protected Entity() { }
}
