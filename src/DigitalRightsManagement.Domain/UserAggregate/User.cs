using DigitalRightsManagement.Common;

namespace DigitalRightsManagement.Domain.UserAggregate;

public class User() : AggregateRoot(Guid.CreateVersion7());
