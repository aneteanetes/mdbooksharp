namespace MdBookSharp.Extensions
{
    internal static class EnumExtensions
    {
        public static IEnumerable<T> GetAllValues<T>(this Type enumType)
        {
            return Enum.GetValues(enumType).Cast<T>();
        }
    }
}
