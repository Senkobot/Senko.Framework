using System.Collections.Generic;
using System.Linq;
using Senko.TestFramework.Linq;

namespace Senko.TestFramework
{
    public static class EnumerableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> enumerable)
        {
            return new AsyncEnumerable<T>(enumerable);
        }
    }
}
