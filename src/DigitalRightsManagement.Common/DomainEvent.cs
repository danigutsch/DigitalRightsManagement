﻿namespace DigitalRightsManagement.Common;

public abstract record DomainEvent()
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
