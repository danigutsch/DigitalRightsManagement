namespace DigitalRightsManagement.Tests.Shared;

public static class EnumerableExtensions
{
    public static T Random<T>(this IEnumerable<T> enumerable)
    {
#pragma warning disable CA5394
        return System.Random.Shared.GetItems([.. enumerable], 1)
            is [var first, ..]
#pragma warning restore CA5394
        ? first
        : throw new InvalidOperationException("Sequence contains no elements");
    }
}
