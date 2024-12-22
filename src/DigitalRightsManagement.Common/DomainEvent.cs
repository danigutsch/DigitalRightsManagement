namespace DigitalRightsManagement.Common;

public abstract class DomainEvent
{
    public DateTimeOffset OccurredOn { get; }
}
