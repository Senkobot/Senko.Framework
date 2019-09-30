using System.Collections.Generic;
using System.Linq;

namespace Senko.Common
{
    public static class LinqExtensions
    {
        public static T Random<T>(this IEnumerable<T> list)
        {
            return list.ToArray().Random();
        }

        public static T Random<T>(this T[] array)
        {
            return array[StaticRandom.Instance.Next(array.Length)];
        }
    }
}
