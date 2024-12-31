using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DigitalRightsManagement.Infrastructure.Persistence.Converters;

internal sealed class ListOfGuidComparer()
    : ValueComparer<List<Guid>>(
        (e1, e2) => e1!.SequenceEqual(e2!),
        e => e.Aggregate(0, HashCode.Combine),
        e => e.Select(v => v).ToList());
