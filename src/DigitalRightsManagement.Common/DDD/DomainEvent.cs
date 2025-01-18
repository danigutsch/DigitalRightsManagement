using MediatR;

namespace DigitalRightsManagement.Common.DDD;

public abstract record DomainEvent : INotification
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
