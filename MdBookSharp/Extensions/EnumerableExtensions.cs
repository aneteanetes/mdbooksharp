namespace MdBookSharp.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static bool Contains<T>(this IEnumerable<T> collection, params T[] values) 
            => collection.Any(values.Contains);

        internal static bool Contains(this string str, params string[] values)
            => values.Any(x => str.Contains(x, StringComparison.InvariantCulture));
    }
}
