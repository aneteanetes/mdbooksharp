namespace MdBookSharp.Extensions
{
    internal static class ForEachExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T> action)
        {
            foreach (var item in @enum)
            {
                action(item);
            }
        }
    }
}
