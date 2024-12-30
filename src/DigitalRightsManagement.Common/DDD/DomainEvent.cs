namespace DigitalRightsManagement.Common.DDD;

public abstract record DomainEvent()
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
