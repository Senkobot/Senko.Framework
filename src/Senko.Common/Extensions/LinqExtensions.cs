using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

    public static class StaticRandom
    {
        private static int _seed;
        private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        static StaticRandom()
        {
            _seed = Environment.TickCount;
        }

        public static Random Instance => ThreadLocal.Value;
    }
}
